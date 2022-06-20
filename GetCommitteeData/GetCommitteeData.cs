using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Polly;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Threading.Tasks;
using System.Net.Http;
using Polly.RateLimit;

namespace FECIngest
{
    public class GetCommitteeData
    //get detailed committee data for each candidate, uses poly to rate limit requests because of limitations in free API (1000 requests per hour)
    {
        private const string apiKey = "xT2E5C0eUKvhVY74ylbGf4NWXz57XlxTkWV9pOwu";
        [FunctionName("GetCommitteeData")]
        public async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //define polly error handling
            var rateLimit = Policy
                .RateLimitAsync(10, TimeSpan.FromSeconds(60),retr);
            var retry = Policy
                .Handle<RateLimitRejectedException>()
                
                .WaitAndRetryAsync(3, sleepDurationProvider: s => TimeSpan.FromMinutes(1));

            var limitandRetry = Policy.WrapAsync(rateLimit, retry);
                
            
            QueueClient queueClient = new QueueClient("UseDevelopmentStorage=true", "committeeprocess");
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            QueueMessage[] candidateIDs = await queueClient.ReceiveMessagesAsync(32);
            CommitteeSearcher committeeSearcher = new CommitteeSearcher(apiKey);
            //process candidate IDs by looking up candidate ID from queue message using FEC API, write data to table storage
            foreach (var candidate in candidateIDs)
            {
                committeeSearcher.SetCandidate(candidate.Body.ToString());
                //try
                //{
                    bool result = await rateLimit.ExecuteAsync(() => committeeSearcher.Submit());
                    if (!result)
                    {
                        log.LogInformation("problem retrieving CandidateIds from queue for processing");
                    }
                    else
                    {
                        foreach (var committee in committeeSearcher.Committees)
                        {
                            //dates written to azure table storage must be UTC
                            var fixedCommittee = committee.AddUTC();
                            TableEntity committeeEntity = fixedCommittee.ToTable(tableClient, "Committee", Guid.NewGuid().ToString());
                            var errorState = await tableClient.AddEntityAsync(committeeEntity);
                            if (errorState.IsError)
                            {
                                log.LogInformation("Problem writing committee to storage for {1}", candidate.Body.ToString());
                            }
                            else //change CommitteeProcessed state to true, remove message from queue
                            {

                            }
                        }
                    }
                //}
                //catch (RateLimitRejectedException ex)
                //{

                //}
                
                

            }
        }
    }
}

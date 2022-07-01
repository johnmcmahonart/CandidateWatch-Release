using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.RateLimit;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FECIngest
{
    public class GetCommitteeData
    //get detailed committee data for each candidate, uses poly to rate limit requests because of limitations in free API (1000 requests per hour)
    {
        private const string apiKey = "xT2E5C0eUKvhVY74ylbGf4NWXz57XlxTkWV9pOwu";

        [FunctionName("GetCommitteeData")]
        public async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
                        
            QueueClient queueClient = new QueueClient("UseDevelopmentStorage=true", "committeeprocess");
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            QueueMessage[] candidateIDs = await queueClient.ReceiveMessagesAsync(32);
            CommitteeSearch committeeSearch = new CommitteeSearch(apiKey);
            //process candidate IDs by looking up candidate ID from queue message using FEC API, write data to table storage
            
            foreach (var candidate in candidateIDs)
            {
                committeeSearch.SetQuery(new Dictionary<string, string>() 
                {
                    {
                     "candidateId", candidate.Body.ToString()
                    } }
                );

                log.LogInformation("Getting committee information for candidate: {1}", candidate.Body.ToString());
                bool result = await SharedComponents.PollyPolicy.GetDefault.ExecuteAsync(() => committeeSearch.Submit());
                if (!result)
                {
                    log.LogInformation("problem retrieving CandidateIds from queue for processing");
                }
                else
                {
                    TableEntity entity = await tableClient.GetEntityAsync<TableEntity>("Candidate", candidate.Body.ToString());
                    entity["CommitteeProcessed"] = true;
                    await tableClient.UpdateEntityAsync(entity, entity.ETag);
                    await queueClient.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
                    foreach (var committee in committeeSearch.Committees)
                    {
                        //dates written to azure table storage must be UTC
                        var fixedCommittee = committee.AddUTC();
                        TableEntity committeeEntity = fixedCommittee.ToTable(tableClient, "Committee", Guid.NewGuid().ToString());
                        var errorState = await tableClient.AddEntityAsync(committeeEntity);


                        if (errorState.IsError) //schedule candidate to be processed later
                        {
                            //todo handle duplicate committee's during reprocessing
                            log.LogInformation("Problem writing committee to storage for {1}", candidate.Body.ToString());
                            TableEntity failed = await tableClient.GetEntityAsync<TableEntity>("Candidate", candidate.Body.ToString());
                            entity["CommitteeProcessed"] = false;
                            await tableClient.UpdateEntityAsync(failed, failed.ETag);
                        }

                    }

                }
            }
        }
    }
}
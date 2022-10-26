using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MDWatch.Model;
using MDWatch.SolutionClients;
using MDWatch.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MDWatch
{
    public class GetCommitteeData
    //get detailed committee data for each candidate, uses poly to rate limit requests because of limitations in free API (1000 requests per hour)
    {
        private static string apiKey { get => General.GetFECAPIKey(); }
        

        [FunctionName("GetCommitteeData")]
        public async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");



            QueueClient queueClient = AzureUtilities.GetQueueClient(General.EnvVars["queue_committee"].ToString());
            QueueMessage[] candidateIDs = await queueClient.ReceiveMessagesAsync(32);
            
            
            
            CommitteeSearchClient committeeSearch = new CommitteeSearchClient(apiKey);
            //process candidate IDs by looking up candidate ID from queue message using FEC API, write data to table storage

            foreach (var candidate in candidateIDs)
            {
                CandidateQueueMessage queueMessage = AzureUtilities.ParseCandidateQueueMessage(candidate.Body.ToString());
                TableClient tableClient = AzureUtilities.GetTableClient(queueMessage.State);
                committeeSearch.SetQuery(new FECQueryParms { CandidateId = queueMessage.CandidateId });

                log.LogInformation("Getting committee information for candidate: {1}", queueMessage.CandidateId);
                try
                {
                    await committeeSearch.SubmitAsync();
                    TableEntity entity = await tableClient.GetEntityAsync<TableEntity>(General.EnvVars["partition_candidate"].ToString(), queueMessage.CandidateId);
                    entity["CommitteeProcessed"] = true;
                    await tableClient.UpdateEntityAsync(entity, entity.ETag);
                    await queueClient.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
                    foreach (var committee in committeeSearch.Committees)
                    {
                        //dates written to azure table storage must be UTC
                        var fixedCommittee = committee.AddUTC(); //TODO this isn't needed anymore
                        TableEntity committeeEntity = fixedCommittee.ModelToTableEntity(tableClient, General.EnvVars["partition_committee"].ToString(), Guid.NewGuid().ToString());
                        var errorState = await tableClient.AddEntityAsync(committeeEntity);

                        if (errorState.IsError) //schedule candidate to be processed later
                        {
                            //todo handle duplicate committee's during reprocessing
                            log.LogInformation("Problem writing committee to storage for {1}", queueMessage.CandidateId);
                            TableEntity failed = await tableClient.GetEntityAsync<TableEntity>(General.EnvVars["partition_candidate"].ToString(), queueMessage.CandidateId);
                            entity["CommitteeProcessed"] = false;
                            await tableClient.UpdateEntityAsync(failed, failed.ETag);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.LogInformation(ex.ToString());
                    log.LogInformation("problem retrieving CandidateIds from queue for processing");
                }
            }
        }
    }
}
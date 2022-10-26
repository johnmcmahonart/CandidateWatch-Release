using System;
using System.Collections.Generic;
using MDWatch.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using Azure;
using MDWatch.Utilities;
using System.Threading.Tasks;
using Polly;
using System.Collections.Concurrent;
using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace MDWatch
{
    public class BuildCandidatebyYearPartition
    {
        //builds partition of candidates grouped by year
        private static string apiKey { get => General.GetFECAPIKey(); }
        
        
        [FunctionName("BuildCandidatebyYearPartition")]

        //this function is used to optimize retrieval of candidates grouped by date for UI and repository
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            QueueClient uiQueueClient = AzureUtilities.GetQueueClient(General.EnvVars["queue_ui_build"].ToString());
            QueueMessage[] uiMessages = await uiQueueClient.ReceiveMessagesAsync(2);
            foreach (var state in uiMessages)
            {
                //clear table so data can be regenerated
                TablePurge.Purge(General.EnvVars["partition_candidate_by_year"].ToString(), state.Body.ToString());
                List<Candidate> allCandidatesModel = new();

                TableClient tableClient = AzureUtilities.GetTableClient(state.Body.ToString());
                

                AsyncPageable<TableEntity> candidates = tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{General.EnvVars["partition_candidate"].ToString()}'");

                //convert table entity to model
                await foreach (var candidate in candidates)
                {
                    allCandidatesModel.Add(candidate.TableEntityToModel<Candidate>());
                }

                //sort model
                IEnumerable<CandidatebyYear> sortedCandidates = CandidateSort.Year((IEnumerable<Candidate>)allCandidatesModel);

                //write to table storage

                try
                {
                    foreach (var year in sortedCandidates)
                    {
                        await tableClient.AddEntityAsync<TableEntity>(year.ModelToTableEntity(tableClient, General.EnvVars["partition_candidate_by_year"].ToString(), year.Year.ToString()));
                    }
                    await uiQueueClient.DeleteMessageAsync(state.MessageId, state.PopReceipt);
                    log.LogInformation("Generated UI data for {1}", state.Body.ToString());
                }
                catch
                {
                    log.LogInformation("Problem generating UI data for {1}", state.Body.ToString());
                }
            }
            
            
            
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            return new OkResult();
        }
    }
}

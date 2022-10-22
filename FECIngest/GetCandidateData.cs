using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MDWatch.Model;
using MDWatch.SolutionClients;
using MDWatch.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SharedComponents.Models;
using Polly;
using GetCandidateData.Model;

namespace MDWatch

{
    public class GetCandidateData
    {
        private static string apiKey { get => General.GetFECAPIKey(); }

        [FunctionName("GetCandidateData")]
        
        public static async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)

        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            


            QueueClient candidateQueueClient = AzureUtilities.GetQueueClient(General.EnvVars["queue_candidate"].ToString());
            
            QueueMessage[] candidatePages = await candidateQueueClient.ReceiveMessagesAsync(32);
            var totalCandidates = 0;
            var totalFailures = 0;
            //process each page from queue to retrieve candidate data, write to table storage, and create candidate status entry
            foreach (var page in candidatePages)
            {
                StateCandidatesQueueMessage messageData = AzureUtilities.ParseStateCandidatesQueueMessage(page.Body.ToString());
                //find all candidates for state
                CandidateSearchClient stateCandidates = new CandidateSearchClient(apiKey, messageData.State);
                
                TableClient tableClient = AzureUtilities.GetTableClient(messageData.State);
                stateCandidates.SetPage(messageData.Page);
                await stateCandidates.SubmitAsync();

                try
                {
                    foreach (var candidate in stateCandidates.Candidates)
                    {
                        totalCandidates++;
                        TableEntity candidateEntity = candidate.ModelToTableEntity(tableClient, General.EnvVars["partition_candidate"].ToString(), candidate.CandidateId);
                        CandidateStatus candidateStatus = new CandidateStatus() { CandidateId = candidate.CandidateId };
                        TableEntity candidateStatusEntity = candidateStatus.ModelToTableEntity(tableClient, General.EnvVars["partition_candidate_status"].ToString(), candidate.CandidateId);
                        try
                        {
                            await tableClient.AddEntityAsync(candidateEntity);
                            //await queueClient.SendMessageAsync(AzureUtilities.MakeCandidateQueueMessage(candidate.CandidateId, state));
                            await tableClient.AddEntityAsync(candidateStatusEntity);
                        }
                        catch (Exception ex)
                        {
                            log.LogInformation("Problem writing candidate to table:{1}", candidate.Name);
                            totalFailures++;
                        }
                    }
                    //no errors, remove message from queue
                    await candidateQueueClient.DeleteMessageAsync(page.MessageId, page.PopReceipt);
                }
                catch
                {
                    log.LogInformation("Problem with page {1} during table write", messageData.Page.ToString());
                }

                log.LogInformation("Processed {1} candidates for {2}, out of {3} possible", totalCandidates-totalFailures, messageData.State, totalCandidates);
            }

            
        }
    }
}
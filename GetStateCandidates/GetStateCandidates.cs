using System;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MDWatch.SolutionClients;
using MDWatch.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MDWatch
{
    public static class GetStateCandidates
    {
        private static string apiKey { get => General.GetFECAPIKey(); }

        [FunctionName("GetStateCandidates")]
        public static async Task<IActionResult> Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            QueueClient stateQueueClient = AzureUtilities.GetQueueClient(General.EnvVars["queue_state_candidate"].ToString());
            QueueClient candidateQueueClient = AzureUtilities.GetQueueClient(General.EnvVars["queue_candidate"].ToString());
            //find all candidates for state
            QueueMessage[] states = await stateQueueClient.ReceiveMessagesAsync(10);
            foreach (var item in states)
            {
                string state = item.Body.ToString();
                CandidateSearchClient stateCandidates = new CandidateSearchClient(apiKey, state);
                stateCandidates.SetPage(1);
                await stateCandidates.SubmitAsync();
                log.LogInformation("Found {1} pages of candidates for {2}", stateCandidates.TotalPages, state);

                //write candidate page numbers to queue for processing by downstream worker

                for (int i = 1; i < stateCandidates.TotalPages + 1; i++)
                {
                    try
                    {
                        await candidateQueueClient.SendMessageAsync(AzureUtilities.MakeStateCandidatesQueueMessage(state, i));
                    }
                    catch
                    {
                        log.LogInformation("Problem writing candidate pages to table:Number={1}", i);
                    }
                }
            }
            

            return new OkResult();
        }
    }
}
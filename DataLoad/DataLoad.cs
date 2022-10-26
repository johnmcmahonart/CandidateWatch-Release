using System;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MDWatch.Model;
using MDWatch.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;


namespace MDWatch
{
    //handles loading the majority of the data for the solution when it is run for the first time
    public class DataLoad
    {
        [FunctionName("DataLoad")]
        
        public static async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //get candidates from dataload queue and create messages for downstream workers to load data into the solution
            QueueClient dataLoadQueueClient = AzureUtilities.GetQueueClient(General.EnvVars["queue_data_load"].ToString());

            QueueMessage[] candidates = await dataLoadQueueClient.ReceiveMessagesAsync(32);
            foreach (var candidate in candidates)
            {
                CandidateQueueMessage queueMessage = AzureUtilities.ParseCandidateQueueMessage(candidate.Body.ToString());
                try
                {
                    QueueClient committeeQueue = AzureUtilities.GetQueueClient(General.EnvVars["queue_committee"].ToString());
                    await committeeQueue.SendMessageAsync(candidate.Body.ToString());
                    QueueClient financeQueue = AzureUtilities.GetQueueClient(General.EnvVars["queue_finance_totals"].ToString());
                    await financeQueue.SendMessageAsync(candidate.Body.ToString());
                    QueueClient scheduleBQueue = AzureUtilities.GetQueueClient(General.EnvVars["queue_scheduleb_candidate"].ToString());
                    await scheduleBQueue.SendMessageAsync(candidate.Body.ToString());
                    await dataLoadQueueClient.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
                    log.LogInformation("Data Load for {1} completed",queueMessage.CandidateId);
                }
                catch
                {
                    log.LogInformation("Problem with Data Load for {1}", queueMessage.CandidateId);
                }




            }


        }
    }
}
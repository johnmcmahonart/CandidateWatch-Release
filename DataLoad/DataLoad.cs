using System;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MDWatch.Model;
using MDWatch.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MDWatch
{
    //handles loading the majority of the data for the solution when it is run for the first time
    public class DataLoad
    {
        [FunctionName("DataLoad")]
        public static async Task Run([QueueTrigger("dataloadprocess")] QueueMessage candidateMessage, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //get candidates from dataload queue and create messages for downstream workers to load data into the solution
            CandidateQueueMessage candidate = AzureUtilities.ParseCandidateQueueMessage(candidateMessage.Body.ToString());
            QueueClient dataLoadQueueClient = AzureUtilities.GetQueueClient(General.EnvVars["queue_data_load"].ToString());
            try
            {
                QueueClient committeeQueue = AzureUtilities.GetQueueClient(General.EnvVars["queue_committee"].ToString());
                await committeeQueue.SendMessageAsync(candidateMessage.Body.ToString());
                QueueClient financeQueue = AzureUtilities.GetQueueClient(General.EnvVars["queue_finance_totals"].ToString());
                await financeQueue.SendMessageAsync(candidateMessage.Body.ToString());
                QueueClient scheduleBQueue = AzureUtilities.GetQueueClient(General.EnvVars["queue_scheduleb_candidate"].ToString());
                await scheduleBQueue.SendMessageAsync(candidateMessage.Body.ToString());
                await dataLoadQueueClient.DeleteMessageAsync(candidateMessage.MessageId, candidateMessage.PopReceipt);
                log.LogInformation("Data Load for {1} completed", candidate.CandidateId);
            }
            catch
            {
                log.LogInformation("Problem with Data Load for {1}", candidate.CandidateId);
            }
        }
    }
}

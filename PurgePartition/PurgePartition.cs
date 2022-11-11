using System;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MDWatch.Model;
using MDWatch.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MDWatch
{
    


    public class PurgePartition
    {
        [FunctionName("PurgePartition")]

        
        public static async Task Run([QueueTrigger("purgeprocess")] QueueMessage purgeMessage, ILogger log)
        {
            QueueClient purgeProcessQueueClient = AzureUtilities.GetQueueClient(General.EnvVars["queue_purge"].ToString());

            CandidateQueueMessage decodedMessage = JsonConvert.DeserializeObject<CandidateQueueMessage>(purgeMessage.ToString());

            log.LogInformation("C# Queue trigger function processed: {1}:{2}",decodedMessage.CandidateId,decodedMessage.State);
            string result = TablePurge.Purge(decodedMessage.CandidateId, decodedMessage.State) ? "Purge partiton completed successfully" : "Problem purging partition";
            log.LogInformation(result);
            await purgeProcessQueueClient.DeleteMessageAsync(purgeMessage.MessageId, purgeMessage.PopReceipt);
        }
    }
}

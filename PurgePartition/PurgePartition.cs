using System;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MDWatch.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
namespace MDWatch
{
    public class PurgePartition
    {
        [FunctionName("PurgePartition")]

        
        public static async Task Run([QueueTrigger("purgeprocess")] QueueMessage purgeMessage, ILogger log)
        {
            QueueClient purgeProcessQueueClient = AzureUtilities.GetQueueClient(General.EnvVars["queue_purge"].ToString());

            var splitMessage = purgeMessage.Body.ToString().Split(',');

            log.LogInformation("C# Queue trigger function processed: {1}:{2}", splitMessage[0], splitMessage[1]);
            string result = TablePurge.Purge(splitMessage[0], splitMessage[1]) ? "Purge partiton completed successfully" : "Problem purging partition";
            log.LogInformation(result);
            await purgeProcessQueueClient.DeleteMessageAsync(purgeMessage.MessageId, purgeMessage.PopReceipt);
        }
    }
}

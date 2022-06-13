using System;
using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.Storage.Queues;
using System.Linq;
using FECIngest.Model;
using System.Threading.Tasks;

namespace FECIngest
{
    public class SendUpdates
    {
        [FunctionName("SendUpdates")]
        public async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            //get entries from candidate partition that haven't been processed
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            QueueClient queueClient = new QueueClient("UseDevelopmentStorage=true", "committeeprocess");
            Pageable<TableEntity> queryResult =tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'Candidate' and CommitteeProcessed eq false");
            if (queryResult.Count()>0)
            {
                log.LogInformation("Found {1} candidates missing committee information: ",queryResult.Count());
                //write messages to queue for look up later, we only need the candidate ID to perform the lookup from the FEC API
                foreach (var row in queryResult)
                {
                    object candidateID = new object();
                    row.TryGetValue(UtilityExtensions.GetMemberName((Candidate c) => c.CandidateId), out candidateID);
                    await queueClient.SendMessageAsync(candidateID.ToString());
                }
                
            
            }



        }
    }
}

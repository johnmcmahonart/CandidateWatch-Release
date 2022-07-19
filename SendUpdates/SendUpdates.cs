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
            QueueClient committeeQueue = new QueueClient("UseDevelopmentStorage=true", "committeeprocess");
            Pageable<TableEntity> committeeQuery =tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'Candidate' and CommitteeProcessed eq false");
            if (committeeQuery.Count()>0)
            {
                log.LogInformation("Found {1} candidates missing committee information: ",committeeQuery.Count());
                //write messages to queue for look up later, we only need the candidate ID to perform the lookup from the FEC API
                foreach (var row in committeeQuery)
                {
                    object candidateID = new object();
                    row.TryGetValue(UtilityExtensions.GetMemberName((Candidate c) => c.CandidateId), out candidateID);
                    await committeeQueue.SendMessageAsync(candidateID.ToString());
                }
                
            
            }
            QueueClient financeTotalsQueue = new QueueClient("UseDevelopmentStorage=true", "financetotalsprocess");
            Pageable<TableEntity> financeTotalsQuery = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'Candidate' and FinanceTotalProcessed eq false");
            if (financeTotalsQuery.Count() > 0)
            {
                log.LogInformation("Found {1} candidates missing Financial Total (aggregate) information: ", financeTotalsQuery.Count());
                //write messages to queue for look up later, we only need the candidate ID to perform the lookup from the FEC API
                foreach (var row in financeTotalsQuery)
                {
                    object candidateID = new object();
                    row.TryGetValue(UtilityExtensions.GetMemberName((Candidate c) => c.CandidateId), out candidateID);
                    await financeTotalsQueue.SendMessageAsync(candidateID.ToString());
                }


            }

            QueueClient scheduleBQueue = new QueueClient("UseDevelopmentStorage=true", "schedulebcandidateprocess");
            Pageable<TableEntity> scheduleBQuery = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'Candidate' and ScheduleBProcessed eq false");
            if (scheduleBQuery.Count() > 0)
            {
                log.LogInformation("Found {1} candidates missing ScheduleB information: ", scheduleBQuery.Count());
                //write messages to queue for look up later, we only need the candidate ID to perform the lookup from the FEC API
                foreach (var row in scheduleBQuery)
                {
                    object candidateID = new object();
                    row.TryGetValue(UtilityExtensions.GetMemberName((Candidate c) => c.CandidateId), out candidateID);
                    await scheduleBQueue.SendMessageAsync(candidateID.ToString());
                }


            }
        }
    }
}

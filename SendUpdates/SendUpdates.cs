using System;
using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.Storage.Queues;
using System.Linq;
using MDWatch.Model;
using MDWatch.Utilities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace MDWatch
{
    public class SendUpdates
    {
        [FunctionName("SendUpdates")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)

        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            string state = req.Query["state"];

            
                
            //get entries from candidate partition that haven't been processed
            TableClient tableClient = AzureUtilities.GetTableClient(state);
            QueueClient committeeQueue = AzureUtilities.GetQueueClient(General.EnvVars["queue_committee"].ToString());
            Pageable<TableEntity> committeeQuery =tableClient.Query<TableEntity>(filter: $"PartitionKey eq '{General.EnvVars["partition_candidate_status"].ToString()}' and CommitteeProcessed eq false");
            if (committeeQuery.Count()>0)
            {
                log.LogInformation("Found {1} candidates missing committee information: ",committeeQuery.Count());
                //write messages to queue for look up later, we only need the candidate ID to perform the lookup from the FEC API
                foreach (var row in committeeQuery)
                {
                    object candidateID = new object();
                    row.TryGetValue(Utilities.General.GetMemberName((Candidate c) => c.CandidateId), out candidateID);
                    await committeeQueue.SendMessageAsync(AzureUtilities.MakeCandidateQueueMessage(candidateID.ToString(),state));
                }
                
            
            }
            QueueClient financeTotalsQueue = AzureUtilities.GetQueueClient(General.EnvVars["queue_finance_totals"].ToString());
            Pageable<TableEntity> financeTotalsQuery = tableClient.Query<TableEntity>(filter: $"PartitionKey eq '{General.EnvVars["partition_candidate_status"].ToString()}' and FinanceTotalProcessed eq false");
            if (financeTotalsQuery.Count() > 0)
            {
                log.LogInformation("Found {1} candidates missing Financial Total (aggregate) information: ", financeTotalsQuery.Count());
                //write messages to queue for look up later, we only need the candidate ID to perform the lookup from the FEC API
                foreach (var row in financeTotalsQuery)
                {
                    object candidateID = new object();
                    row.TryGetValue(Utilities.General.GetMemberName((Candidate c) => c.CandidateId), out candidateID);
                    
                    await financeTotalsQueue.SendMessageAsync(AzureUtilities.MakeCandidateQueueMessage(candidateID.ToString(), state));
                }


            }

            QueueClient scheduleBQueue = AzureUtilities.GetQueueClient(General.EnvVars["queue_scheduleb_candidate"].ToString());
            Pageable<TableEntity> scheduleBQuery = tableClient.Query<TableEntity>(filter: $"PartitionKey eq '{General.EnvVars["partition_candidate_status"].ToString}' and ScheduleBProcessed eq false");
            if (scheduleBQuery.Count() > 0)
            {
                log.LogInformation("Found {1} candidates missing ScheduleB information: ", scheduleBQuery.Count());
                //write messages to queue for look up later, we only need the candidate ID to perform the lookup from the FEC API
                foreach (var row in scheduleBQuery)
                {
                    object candidateID = new object();
                    row.TryGetValue(Utilities.General.GetMemberName((Candidate c) => c.CandidateId), out candidateID);
                    await scheduleBQueue.SendMessageAsync(AzureUtilities.MakeCandidateQueueMessage(candidateID.ToString(), state));
                }


            }
            return new OkResult();
        }
    }
}

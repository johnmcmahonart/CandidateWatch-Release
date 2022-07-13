using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ValidateScheduleB
{
    public class ValidateScheduleB
    {
        [FunctionName("ValidateScheduleB")]
        public void Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            //check if table contains all data from API
            //if candidate does not have any disbursement data, still mark as processed and remove from queue
            /*
            Pageable<TableEntity> candidateScheduleB = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'ScheduleB' and CandidateId eq '{candidate.Body.ToString()}'");
            if (candidateScheduleB.Count() == scheduleBDisbursement.TotalDisbursementsforCandidate)
            {
                TableEntity entity = await tableClient.GetEntityAsync<TableEntity>("Candidate", candidate.Body.ToString());
                entity["ScheduleBProcessed"] = true;
                log.LogInformation("Candidate: {1} has been processed", candidate.Body.ToString());
                await tableClient.UpdateEntityAsync(entity, entity.ETag);
                await scheduleBCandidateQueue.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);

            }
            */


        }
    }
}

using Azure;
using Azure.Data.Tables;
using FECIngest.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FECIngest
{
    public class ValidateScheduleB
    {
        [FunctionName("ValidateScheduleB")]
        public async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            //Get all candidates that haven't had scheduleB processed
            //verify if all scheduleB disbursements have been logged to table, if so mark as processed
            //If not, remove any previously downloaded disbursement entries

            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            Pageable<TableEntity> candidatesScheduleB = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'Candidate' and {UtilityExtensions.GetMemberName((Candidate c) => c.ScheduleBProcessed)} eq false");

            

            foreach (var candidate in candidatesScheduleB)
            {
                object foundCandidate = new();
                candidate.TryGetValue(UtilityExtensions.GetMemberName((Candidate c) => c.CandidateId), out foundCandidate);
                Pageable<TableEntity> scheduleBOverview = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'ScheduleBOverview' and RowKey eq '{foundCandidate}'");
                
                Pageable<TableEntity> scheduleBDetail = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'ScheduleBDetail' and {UtilityExtensions.GetMemberName((ScheduleBByRecipientID c) => c.RecipientId)}  eq '{scheduleBOverview.First()["PrincipalCommitteeId"].ToString()}'");
                
                if ((int)scheduleBOverview.First()["TotalDisbursements"] == scheduleBDetail.Count())
                {
                    candidate[UtilityExtensions.GetMemberName((Candidate c) => c.ScheduleBProcessed)] = true;
                    await tableClient.UpdateEntityAsync(candidate, candidate.ETag);
                    log.LogInformation("{1} has correct number of ScheduleB disbursements stored", candidate["CandidateId"]);
                }
                else
                {
                    log.LogInformation("Cleaning up previously downloaded data for {1}. Will try downloading ScheduleB data at a later time",foundCandidate);
                    foreach (var disbursement in scheduleBDetail)
                    {
                        //remove partially downloaded disbursements, so when we retry later there aren't any duplicates
                        await tableClient.DeleteEntityAsync(disbursement.PartitionKey, disbursement.RowKey);
                    }
                }
            }
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
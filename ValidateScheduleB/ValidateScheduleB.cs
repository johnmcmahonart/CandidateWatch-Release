using System;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using MDWatch.Model;
using MDWatch.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MDWatch
{
    public class ValidateScheduleB
    {
        [FunctionName("ValidateScheduleB")]
        public async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            //Get all candidates that haven't had scheduleB processed
            //verify if all scheduleB disbursements have been logged to table, if so mark as processed
            //If not, remove any previously downloaded disbursement entries

            int totalCorrect = 0;
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");

            Pageable<TableEntity> candidatesScheduleBNotProcessed = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'CandidateStatus' and {Utilities.General.GetMemberName((CandidateStatus c) => c.ScheduleBProcessed)} eq false");

            foreach (var candidate in candidatesScheduleBNotProcessed)
            {
                string candidateId = (string)candidate[Utilities.General.GetMemberName((ScheduleBCandidateOverview c) => c.CandidateId)];
                TableEntity scheduleBOverview = await tableClient.GetEntityAsync<TableEntity>("ScheduleBOverview", candidateId);
                string committeeId = (string)scheduleBOverview[Utilities.General.GetMemberName((ScheduleBCandidateOverview c) => c.PrincipalCommitteeId)];
                Pageable<TableEntity> scheduleBDetail = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'ScheduleBDetail' and {Utilities.General.GetMemberName((ScheduleBByRecipientID c) => c.RecipientId)}  eq '{committeeId}'");

                if ((int)scheduleBOverview[Utilities.General.GetMemberName((ScheduleBCandidateOverview c) => c.TotalDisbursements)] == scheduleBDetail.Count())
                {
                    TableEntity candidateScheduleBProcessed = await tableClient.GetEntityAsync<TableEntity>("CandidateStatus", candidateId);
                    candidateScheduleBProcessed[Utilities.General.GetMemberName((CandidateStatus c) => c.ScheduleBProcessed)] = true;
                    await tableClient.UpdateEntityAsync(candidateScheduleBProcessed, candidateScheduleBProcessed.ETag);
                    //log.LogInformation("{1} has correct number of ScheduleB disbursements stored", (string)candidate.CandidateId);
                    totalCorrect++;
                }
                else
                {
                    log.LogInformation("{1} has {2} out of {3} ScheduleB entries downloaded", candidateId, scheduleBDetail.Count(), scheduleBOverview[Utilities.General.GetMemberName((ScheduleBCandidateOverview c) => c.TotalDisbursements)]);
                    log.LogInformation("Cleaning up previously downloaded data for {1}. Will try downloading ScheduleB data at a later time", candidateId);

                    //remove overview data since it may change between the next run of getting the overview information, which can cause the API and DB to disagree
                    await tableClient.DeleteEntityAsync("ScheduleBOverview", candidateId);
                    foreach (var disbursement in scheduleBDetail)
                    {
                        //remove partially downloaded disbursements, so when we retry later there aren't any duplicates
                        await tableClient.DeleteEntityAsync(disbursement.PartitionKey, disbursement.RowKey);
                    }
                }
            }

            log.LogInformation("Found {1} of {2} candidates that have correct amount of scheduleB disbursements recorded", totalCorrect, candidatesScheduleBNotProcessed.Count().ToString());
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
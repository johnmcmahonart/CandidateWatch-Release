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
            Pageable<TableEntity> candidatesScheduleBOverview = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'ScheduleBOverview'");
            Pageable<TableEntity> candidatesScheduleBProcessed = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'Candidate' and {UtilityExtensions.GetMemberName((Candidate c) => c.ScheduleBProcessed)} eq false");

            
            
            var scheduleBNotProcessedQuery = from isProcessed in candidatesScheduleBProcessed
                                             join candidateOverview in candidatesScheduleBOverview on isProcessed[UtilityExtensions.GetMemberName((Candidate c) => c.CandidateId)] equals candidateOverview[UtilityExtensions.GetMemberName((Candidate c) => c.CandidateId)]
                                             select new
                                             {
                                                 CandidateId = candidateOverview[UtilityExtensions.GetMemberName((Candidate c) => c.CandidateId)],
                                                 PrincipalCommitteeId = candidateOverview[UtilityExtensions.GetMemberName((ScheduleBCandidateOverview c) => c.PrincipalCommitteeId)],
                                                 TotalDisbursements = candidateOverview[UtilityExtensions.GetMemberName((ScheduleBCandidateOverview c) => c.TotalDisbursements)]
                                             };
            
            foreach (var candidate in scheduleBNotProcessedQuery)
            {
                
                //object foundCandidate = new();
                //candidate.TryGetValue(UtilityExtensions.GetMemberName((Candidate c) => c.CandidateId), out foundCandidate);
                //TableEntity scheduleBOverview = await tableClient.GetEntityAsync<TableEntity>("ScheduleBOverview", foundCandidate.ToString());

                Pageable<TableEntity> scheduleBDetail = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'ScheduleBDetail' and {UtilityExtensions.GetMemberName((ScheduleBByRecipientID c) => c.RecipientId)}  eq '{(string)candidate.PrincipalCommitteeId}'");

                if ((int)candidate.TotalDisbursements == scheduleBDetail.Count())
                {
                    TableEntity candidateScheduleBProcessed = await tableClient.GetEntityAsync < TableEntity> ("Candidate", (string)candidate.CandidateId);
                    candidateScheduleBProcessed[UtilityExtensions.GetMemberName((Candidate c) => c.ScheduleBProcessed)] = true;
                    await tableClient.UpdateEntityAsync(candidateScheduleBProcessed, candidateScheduleBProcessed.ETag);
                    log.LogInformation("{1} has correct number of ScheduleB disbursements stored", (string)candidate.CandidateId);
                }
                else
                {
                    log.LogInformation("{1} has {2} out of {3} ScheduleB entries downloaded", candidate.CandidateId, scheduleBDetail.Count(), candidate.TotalDisbursements);
                    log.LogInformation("Cleaning up previously downloaded data for {1}. Will try downloading ScheduleB data at a later time", candidate.CandidateId);
                    
                    //remove overview data since it may change between the next run of getting the overview information, which can cause the API and DB to disagree
                    await tableClient.DeleteEntityAsync("ScheduleBOverview", (string)candidate.CandidateId);
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
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using FECIngest.SolutionClients;
using FECIngest.Model;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Azure;
using FECIngest;
namespace Tests
{
    public class ScheduleBValidation
    {
        private const string apiKey = "xT2E5C0eUKvhVY74ylbGf4NWXz57XlxTkWV9pOwu";
        [FunctionName("ScheduleBValidation")]

        public async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            //validate all disbursement information is being retrived and written to table

            
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            Pageable<TableEntity> scheduleBOverview = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'ScheduleBOverview'");

            //sample %10 of entries in ScheduleBOverview for validity
            var rand = new Random();
            int sampleCount = 0;
            int totalScheduleBCorrect = 0;
            foreach (var candidate in scheduleBOverview)
            {
                if (rand.Next(0, 99) < 9)
                {
                    sampleCount++;
                    object foundCandidate = new();
                    candidate.TryGetValue(UtilityExtensions.GetMemberName((ScheduleBCandidateOverview c) => c.CandidateId), out foundCandidate);
                    Pageable<TableEntity> scheduleBDetail =tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'ScheduleBDetail' and {UtilityExtensions.GetMemberName((ScheduleBByRecipientID c) => c.RecipientId)}  eq '{candidate["PrincipalCommitteeId"]}'");
                    int totalScheduleBRows = 0;
                    foreach (var scheduleBRow in scheduleBDetail)
                    {
                        totalScheduleBRows++;
                    }
                    if ((int)candidate["TotalDisbursements"] == totalScheduleBRows)
                    {
                        log.LogInformation("{1} has correct number of ScheduleB disbursements stored", candidate["CandidateId"]);
                        totalScheduleBCorrect++;
                    }
                    else
                    {
                        log.LogInformation("{1} does not have correct number of ScheduleB disbursements stored", candidate["CandidateId"]);
                    }

                }
                
            }
            log.LogInformation("Found {1} out of {2} candidates with correct number of ScheduleB disbursements", totalScheduleBCorrect, sampleCount);
        }
    }
}

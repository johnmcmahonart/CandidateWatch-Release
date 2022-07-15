using Azure.Data.Tables;
using FECIngest.SolutionClients;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FECIngest
{
    public class GetFECData
    {
        private const string apiKey = "xT2E5C0eUKvhVY74ylbGf4NWXz57XlxTkWV9pOwu";
        
        
        [FunctionName("FECIngest")]
        public static async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            //dev stage only
            string[] tableParittions = { "Candidate", "FinanceTotals", "ScheduleBOverview", "ScheduleBDetail" };
            //string[] tableParittions = { "Candidate","ScheduleBOverview" };
            foreach (var partition in tableParittions)
            {
                log.LogInformation("Purging {1} table", partition);
                string result = TablePurge.Purge(partition) ? "Purge partition succeeded": "Problem purging partition";
                log.LogInformation(result);
            }
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //reset solution tables
            
            string r2 = TablePurge.Purge("FinanceTotals") ? "Purge FinanceTotals partition succeeded" : "Problem purging FinanceTotals partition ";
            log.LogInformation(r2);
            string r3 = TablePurge.Purge("ScheduleBOverview") ? "Purge ScheduleB Overview partition succeeded" : "Problem purging ScheduleB partition ";
            log.LogInformation(r3);

            
            //find all candidates for MD
            CandidateSearchClient mdCandidates = new CandidateSearchClient(apiKey, "MD");
            await mdCandidates.SubmitAsync();
            log.LogInformation("Found {1} candidates.", mdCandidates.Candidates.Count);
            //save candidate data to table storage
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");

            foreach (var candidate in mdCandidates.Candidates)
            {
                //candidate.ToTableEntity
                var fixedCandidate = candidate.AddUTC();
                TableEntity candidateEntity = fixedCandidate.ToTable(tableClient, "Candidate", candidate.CandidateId);
                try
                {
                    var status = await tableClient.AddEntityAsync(candidateEntity);
                }
                catch (Exception ex)
                {
                    log.LogInformation("Problem writing candidate to table:{1}", candidate.Name);
                }
                
            }

            
        }
    }
}
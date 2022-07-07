using Azure.Data.Tables;
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
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //reset solution tables
            string r1 = TablePurge.Purge("Candidate") ? "Purge candidate table succeeded" : "Problem purging candidate table ";
            log.LogInformation(r1);
            string r2 = TablePurge.Purge("FinanceTotals") ? "Purge FinanceTotals table succeeded" : "Problem purging FinanceTotals table ";
            log.LogInformation(r2);
            
            //find all candidates for MD
            CandidateSearch mdCandidates = new CandidateSearch(apiKey, "MD");
            await mdCandidates.Submit();
            log.LogInformation("Found {1} candidates.", mdCandidates.Candidates.Count);
            //save candidate data to table storage
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");

            foreach (var candidate in mdCandidates.Candidates)
            {
                //candidate.ToTableEntity
                var fixedCandidate = candidate.AddUTC();
                TableEntity candidateEntity = fixedCandidate.ToTable(tableClient, "Candidate", candidate.CandidateId);
                bool processed = default(bool);
                //add a column to track if committee data for the candidate has been downloaded
                candidateEntity.Add("CommitteeProcessed", processed);
                candidateEntity.Add("FinanceTotalProcessed", processed);
                candidateEntity.Add("ScheduleBProcessed", processed);
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
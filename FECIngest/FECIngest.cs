using Azure.Data.Tables;
using FECIngest.SolutionClients;
using FECIngest.Utilities;
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
        public static async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

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
                TableEntity candidateEntity = fixedCandidate.ModelToTableEntity(tableClient, "Candidate", candidate.CandidateId);
                try
                {
                    await tableClient.AddEntityAsync(candidateEntity);
                }
                catch (Exception ex)
                {
                    log.LogInformation("Problem writing candidate to table:{1}", candidate.Name);
                }
            }
        }
    }
}
using Azure.Data.Tables;
using MDWatch.SolutionClients;
using MDWatch.Utilities;
using MDWatch.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
namespace MDWatch

{
    public class GetCandidateData
    {
        private static string apiKey { get => General.GetFECAPIKey(); }

        [FunctionName("GetCandidateData")]
        public static async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
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

                TableEntity candidateEntity = candidate.ModelToTableEntity(tableClient, "Candidate", candidate.CandidateId);
                CandidateStatus candidateStatus = new CandidateStatus() { CandidateId = candidate.CandidateId };
                TableEntity candidateStatusEntity = candidateStatus.ModelToTableEntity(tableClient, "CandidateStatus", candidate.CandidateId);
                try
                {
                    await tableClient.AddEntityAsync(candidateEntity);
                    await tableClient.AddEntityAsync(candidateStatusEntity);
                }
                catch (Exception ex)
                {
                    log.LogInformation("Problem writing candidate to table:{1}", candidate.Name);
                }
            }
        }
    }
}


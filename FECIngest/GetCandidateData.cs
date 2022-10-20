using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using MDWatch.Model;
using MDWatch.SolutionClients;
using MDWatch.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MDWatch

{
    public class GetCandidateData
    {
        private static string apiKey { get => General.GetFECAPIKey(); }

        [FunctionName("GetCandidateData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)

        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            string state = req.Query["state"];

            TableClient tableClient = AzTableUtilitites.GetTableClient(state);

            //find all candidates for state
            CandidateSearchClient stateCandidates = new CandidateSearchClient(apiKey, state);
            await stateCandidates.SubmitAsync();
            log.LogInformation("Found {1} candidates.", stateCandidates.Candidates.Count);

            //save candidate data to table storage
            foreach (var candidate in stateCandidates.Candidates)
            {
                TableEntity candidateEntity = candidate.ModelToTableEntity(tableClient, General.GetConfigurationValue("partition_candidate"), candidate.CandidateId);
                CandidateStatus candidateStatus = new CandidateStatus() { CandidateId = candidate.CandidateId };
                TableEntity candidateStatusEntity = candidateStatus.ModelToTableEntity(tableClient, General.GetConfigurationValue("partition_candidate_status"), candidate.CandidateId);
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

            return new OkResult();
        }
    }
}
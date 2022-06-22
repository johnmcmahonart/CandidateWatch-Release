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
        public static async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //find all candidates for MD
            CandidateSearcher mdCandidates = new CandidateSearcher(apiKey, "MD");
            await mdCandidates.Submit();
            log.LogInformation("Found {1} candidates.", mdCandidates.Candidates.Count);
            //save candidate data to table storage
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");

            foreach (var candidate in mdCandidates.Candidates)
            {
                //candidate.ToTableEntity
                var fixedCandidate = candidate.AddUTC();
                TableEntity candidateEntity = fixedCandidate.ToTable(tableClient, "Candidate", candidate.CandidateId);
                bool committeeProcessed = default(bool);
                //add a column to track if committee data for the candidate has been downloaded
                candidateEntity.Add("CommitteeProcessed", committeeProcessed);

                var status = await tableClient.AddEntityAsync(candidateEntity);
                
                if (status.IsError)
                {
                    log.LogInformation("Problem writing candidate to table:{1}", candidate.Name);
                }
            }

            
        }
    }
}
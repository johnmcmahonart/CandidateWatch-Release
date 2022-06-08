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

                var status = await tableClient.AddEntityAsync(fixedCandidate.ToTable(tableClient, "Candidate", candidate.CandidateId));
                if (status.IsError)
                {
                    log.LogInformation("Problem writing candidate to table:{1}", candidate.Name);
                }
            }

            /*
            //get committee data
            //CommitteeSearcher committeeInfo = new CommitteeSearcher(apiKey);
            //committeeInfo.SetCandidate(mdCandidates.Candidates[1].CandidateId);

            //need to batch requests to reduce calls, use queue or rate limiting to not exceed 1000 requests per hour
            //await committeeInfo.Submit();

            foreach (var candidate in mdCandidates.Candidates)
            {
                //log.LogInformation(candidate.CandidateId);
                if (!string.IsNullOrEmpty(candidate.CandidateId))
                {
                    committeeInfo.SetCandidate(candidate.CandidateId);
                    await committeeInfo.Submit();
                }

                //log.LogInformation(committeeInfo.Committees.Count.ToString());
            }
            log.LogInformation(committeeInfo.Committees.Count.ToString());
            */
        }
    }
}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FECIngest
{
    public class GetFECData
    {
        [FunctionName("FECIngest")]
        public static async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            CandidateSearcher mdCandidates =new CandidateSearcher("MD");
            await mdCandidates.Submit();
            log.LogInformation("Found {1} candidates.", mdCandidates.Candidates.Count);

            


        }
    }
}

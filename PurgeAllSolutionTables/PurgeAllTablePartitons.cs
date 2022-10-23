using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using MDWatch;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MDWatch
{
    public class PurgeAllTablesPartitions
    {
        [FunctionName("PurgeAllSolutionTables")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            string state = req.Query["state"];
        
            
            //dev stage only
            string[] tableParittions = { "Candidate", "FinanceTotals", "ScheduleBOverview", "ScheduleBDetail","Committee","CandidateStatus","FinanceOverview" };
            //string[] tableParittions = { "Candidate","ScheduleBOverview" };
            foreach (var partition in tableParittions)
            {
                log.LogInformation("Purging {1} table", partition);
                string result = TablePurge.Purge(partition, state) ? "Purge partition succeeded" : "Problem purging partition";
                log.LogInformation(result);
            }
            return new OkResult();
        }
    
    }
}

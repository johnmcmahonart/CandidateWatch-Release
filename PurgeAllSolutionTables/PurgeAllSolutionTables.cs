using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using FECIngest;
namespace PurgeAllSolutionTables
{
    public class PurgeAllSolutionTables
    {
        [FunctionName("PurgeAllSolutionTables")]
        public void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            //dev stage only
            string[] tableParittions = { "Candidate", "FinanceTotals", "ScheduleBOverview", "ScheduleBDetail","Committee" };
            //string[] tableParittions = { "Candidate","ScheduleBOverview" };
            foreach (var partition in tableParittions)
            {
                log.LogInformation("Purging {1} table", partition);
                string result = TablePurge.Purge(partition) ? "Purge partition succeeded" : "Problem purging partition";
                log.LogInformation(result);
            }

        }

    }
}

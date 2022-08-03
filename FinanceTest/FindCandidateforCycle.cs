using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Tests
{
    public class FindCandidateforCycle
    {
        [FunctionName("FindCandidateforCycle")]
        public void Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            /*
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            Pageable<TableEntity> scheduleBOverview = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'ScheduleBOverview'");

            //sample %10 of entries in ScheduleBOverview for validity
            var rand = new Random();
            int sampleCount = 0;
            int totalScheduleBCorrect = 0;
            */
        }
    }
}

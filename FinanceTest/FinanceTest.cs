using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FECIngest
{
    public class FinanceTest
    {
        private const string apiKey = "xT2E5C0eUKvhVY74ylbGf4NWXz57XlxTkWV9pOwu";

        [FunctionName("FinanceTest")]
        public async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            CandidateFinanceTotals financeTotals = new CandidateFinanceTotals(apiKey);

            financeTotals.SetCandidate("H2MD08126");

            bool result = await SharedComponents.PollyPolicy.GetDefault.ExecuteAsync(() => financeTotals.Submit());

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
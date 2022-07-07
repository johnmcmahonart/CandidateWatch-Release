using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace FECIngest
{
    public class FinanceTest
    {
        private const string apiKey = "xT2E5C0eUKvhVY74ylbGf4NWXz57XlxTkWV9pOwu";

        [FunctionName("FinanceTest")]
        public async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            CandidateFinanceTotals financeTotals = new CandidateFinanceTotals(apiKey);

            financeTotals.SetQuery(new FECQueryParmsModel{CandidateId ="H2MD08126"}); 

            await SharedComponents.PollyPolicy.GetDefault.ExecuteAsync(() => financeTotals.Submit());
            log.LogInformation(financeTotals.GetTotalIndividualContributions().ToString());
            log.LogInformation(financeTotals.GetTotalNonIndividualContributions().ToString());
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
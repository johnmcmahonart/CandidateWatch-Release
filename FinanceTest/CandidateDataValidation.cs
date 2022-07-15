using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Data.Tables;
using Newtonsoft.Json;
using System.Linq;
using FECIngest.Model;
using FECIngest.SolutionClients;
namespace FECIngest
{
    public class CandidateDataValidation
    {
        private const string apiKey = "xT2E5C0eUKvhVY74ylbGf4NWXz57XlxTkWV9pOwu";

        [FunctionName("CandidateDataValidation")]
        public async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            var candidateId = "H0MD00010";
            //validate finance aggregation is correct

            CandidateFinanceTotalsClient financeTotals = new CandidateFinanceTotalsClient(apiKey);

            financeTotals.SetQuery(new FECQueryParms { CandidateId = candidateId });


            await financeTotals.SubmitAsync();
            log.LogInformation(financeTotals.GetTotalIndividualContributions().ToString());
            log.LogInformation(financeTotals.GetTotalNonIndividualContributions().ToString());
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            
        }
    }
}
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Data.Tables;
using Newtonsoft.Json;
using System.Linq;
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

            CandidateFinanceTotals financeTotals = new CandidateFinanceTotals(apiKey);

            financeTotals.SetQuery(new FECQueryParmsModel { CandidateId = candidateId });


            await SharedComponents.PollyPolicy.GetDefault.ExecuteAsync(() => financeTotals.SubmitAsync());
            log.LogInformation(financeTotals.GetTotalIndividualContributions().ToString());
            log.LogInformation(financeTotals.GetTotalNonIndividualContributions().ToString());
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //validate all disbursement information is being retrived and written to table

            ScheduleBDisbursement disbursement = new ScheduleBDisbursement(apiKey);
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            TableEntity candidateEntity = await tableClient.GetEntityAsync<TableEntity>("Candidate", candidateId);
            //check for empty json string
            if (!String.Equals(candidateEntity.GetString("PrincipalCommittees-json"), "[]"))
            {
                dynamic principalCommittee = JsonConvert.DeserializeObject(candidateEntity.GetString("PrincipalCommittees-json"));

                string committeeId = principalCommittee[0]["committee_id"];

                disbursement.SetQuery(new FECQueryParmsModel { CommitteeId = committeeId });
                log.LogInformation("Getting ScheduleB data for candidate: {1}", candidateEntity.RowKey);
                /*
                try
                {
                    await SharedComponents.PollyPolicy.GetDefault.ExecuteAsync(() => disbursement.Submit());
                    do
                    {
                        await SharedComponents.PollyPolicy.GetDefault.ExecuteAsync(() => disbursement.GetNextPage());
                    } while (!disbursement.GetNextPage().Result.IsLastPage);
                }
                catch (Exception ex)
                {
                    log.LogError(ex.Message);
                    log.LogInformation("Problem retrieving all ScheduleB data for:{1}", candidateEntity.RowKey);
                }

                var committeeDisbursements = from d in disbursement.Disbursements where d.RecipientId.Contains(committeeId) select d;
                log.LogInformation("{1} has { 2} ScheduldeB disbursements", candidateId, committeeDisbursements.Count());
            */
                }
        }
    }
}
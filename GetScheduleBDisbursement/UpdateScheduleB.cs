using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using FECIngest.Model;
using FECIngest.SolutionClients;
using Newtonsoft.Json;
using System.Linq;

namespace GetScheduleBDisbursement
{
    public class UpdateScheduleB
    {
        [FunctionName("UpdateScheduleB")]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            //check if candidate already has overview data, if so we only need to try redownloading the detail data
            TableEntity scheduleBOverview = await tableClient.GetEntityAsync<TableEntity>("ScheduleBOverview", candidate.Body.ToString());
            TableEntity candidateEntity = await tableClient.GetEntityAsync<TableEntity>("Candidate", candidate.Body.ToString());
            if (scheduleBOverview.Count() < 1)
            {

            }
            else
            {
                //get existing overview from storage, write detail messages to queue
                TableEntity scheduleBOverviewEntity = await tableClient.GetEntityAsync<TableEntity>("ScheduleBOverview", candidate.Body.ToString());

                ScheduleBCandidateOverview scheduleBCandidateOverview = new ScheduleBCandidateOverview
                {
                    CandidateId = candidate.Body.ToString(),
                    TotalDisbursements = (int)scheduleBOverviewEntity["TotalDisbursements"],
                    TotalResultPages = (int)scheduleBOverviewEntity["TotalResultPages"],
                    PrincipalCommitteeId = (string)scheduleBOverviewEntity["PrincipalCommitteeId"],
                };
                await GenerateScheduleBDetailMessages(scheduleBCandidateQueue, scheduleBCandidateOverview, candidate, candidateEntity);
            }
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}

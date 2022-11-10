using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Data.Tables;
using MDWatch.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MDWatch
{
    public class PurgeAllTablesPartitions
    {
        [FunctionName("Orchestrator")]
        public static async Task<HttpResponseMessage> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            log.LogInformation($"Started at {DateTime.Now}:Purging all solution Data....");

            string[] tableParittions = { "Candidate", "FinanceTotals", "ScheduleBOverview", "ScheduleBDetail", "Committee", "CandidateStatus", "FinanceOverview" };
            TableClient solutionConfigTableClient = AzureUtilities.GetTableClient(General.EnvVars["table_solution_config"].ToString());
            TableEntity statesEntity = await solutionConfigTableClient.GetEntityAsync<TableEntity>(General.EnvVars["partition_solution_config"].ToString(), "states");

            dynamic states = JsonConvert.DeserializeObject(statesEntity["allStatesJson"].ToString());
            foreach (string state in states)
            {
                //only data partitions

                foreach (var partition in tableParittions)
                {
                    string callMessage = partition + ',' + state;
                    HttpResponseMessage response = await context.CallActivityAsync<HttpResponseMessage>(nameof(PurgeTable), callMessage);

                    log.LogInformation("Purging {1} table", partition);
                }
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        [FunctionName(nameof(PurgeTable))]
        public static HttpResponseMessage PurgeTable([ActivityTrigger] string callMessage, ILogger log)

        {
            var splitMessage = callMessage.Split(',');
            string result = TablePurge.Purge(splitMessage[0], splitMessage[1]) ? "Purge partition succeeded" : "Problem purging partition";

            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        [FunctionName("ResetSolutionData_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
                    [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestMessage req,
                    [DurableClient] IDurableOrchestrationClient starter,
                    ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Orchestrator", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Data.Tables;
using MDWatch.Utilities;
using Microsoft.AspNetCore.Mvc;
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
            HttpResponseMessage response = await context.CallActivityAsync<HttpResponseMessage>(nameof(PurgeAllSolutionTables), "Started processing");
            return response;
        }

        [FunctionName(nameof(PurgeAllSolutionTables))]
        public static async Task<HttpResponseMessage> PurgeAllSolutionTables([ActivityTrigger] string caller, ILogger log)

        {
            log.LogInformation($"Started at {DateTime.Now}:Purging all solution Data....");

            TableClient solutionConfigTableClient = AzureUtilities.GetTableClient(General.EnvVars["table_solution_config"].ToString());
            TableEntity statesEntity = await solutionConfigTableClient.GetEntityAsync<TableEntity>(General.EnvVars["partition_solution_config"].ToString(), "states");

            dynamic states = JsonConvert.DeserializeObject(statesEntity["allStatesJson"].ToString());
            foreach (string state in states)
            {
                //only data partitions stage only
                string[] tableParittions = { "Candidate", "FinanceTotals", "ScheduleBOverview", "ScheduleBDetail", "Committee", "CandidateStatus", "FinanceOverview" };

                foreach (var partition in tableParittions)
                {
                    log.LogInformation("Purging {1} table", partition);
                    string result = TablePurge.Purge(partition, state) ? "Purge partition succeeded" : "Problem purging partition";
                    log.LogInformation(result);
                }

            }
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
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using MDWatch.Utilities;
using Microsoft.AspNetCore.Http;
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

        [FunctionName("PurgeAllTables")]

        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {


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

                    QueueClient purgeQueueClient = AzureUtilities.GetQueueClient(General.EnvVars["queue_purge"].ToString());

                    var bytes = Encoding.UTF8.GetBytes(callMessage);

                    //await dataLoadQueueClient.SendMessageAsync(Convert.ToBase64String(bytes));

                    await purgeQueueClient.SendMessageAsync(Convert.ToBase64String(bytes));
                        log.LogInformation("Purging {1} table", partition);
                    
                }

            }

            return new OkResult();
        }
    }
}
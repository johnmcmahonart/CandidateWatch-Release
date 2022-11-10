using System.Net.Http;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using MDWatch.Model;
using MDWatch.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MDWatch
{
    //this function checks if all candidates have had all necessary data downloaded, and if they don't it creates worker tasks to download the data
    //this function is necessary because data ingestation can take significant time due to FEC API rate limit and the message may expire from the
    //queue before it is downloaded
    public static class GetMissingCandidateData
    {
        [FunctionName("Orchestrator")]
        public static async Task<HttpResponseMessage> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            string Start = await context.WaitForExternalEvent<string>("Start");

            if (Start=="true")
            {
                log.LogInformation("GetMissingCandidateData Invoked from external trigger");
                HttpResponseMessage response = await context.CallActivityAsync<HttpResponseMessage>("Start", "");
                return response;
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        }

        [FunctionName("ProcessStateCandidates")]
        public static async Task<HttpResponseMessage> Start([ActivityTrigger] IDurableActivityContext startContext, ILogger log)

        {
            TableClient solutionConfigTableClient = AzureUtilities.GetTableClient(General.EnvVars["table_solution_config"].ToString());
            TableEntity statesEntity = await solutionConfigTableClient.GetEntityAsync<TableEntity>(General.EnvVars["partition_solution_config"].ToString(), "states");
            QueueClient committeeQueue = AzureUtilities.GetQueueClient(General.EnvVars["queue_committee"].ToString());

            QueueClient financeQueue = AzureUtilities.GetQueueClient(General.EnvVars["queue_finance_totals"].ToString());

            QueueClient scheduleBQueue = AzureUtilities.GetQueueClient(General.EnvVars["queue_scheduleb_candidate"].ToString());

            dynamic states = JsonConvert.DeserializeObject(statesEntity["allStatesJson"].ToString());
            foreach (string state in states)
            {
                TableClient stateTableClient = AzureUtilities.GetTableClient(state);

                AsyncPageable<TableEntity> candidates = stateTableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{General.EnvVars["partition_candidate_status"]}'");
                await foreach (var candidate in candidates)
                {
                    CandidateStatus candidateModel = candidate.TableEntityToModel<CandidateStatus>();
                    if (!candidateModel.ScheduleBProcessed)
                    {
                        string scheduleBMessage = AzureUtilities.MakeCandidateQueueMessage(candidateModel.CandidateId, state);
                        await scheduleBQueue.SendMessageAsync(scheduleBMessage);
                        log.LogInformation("{1} missing ScheduleB information", candidateModel.CandidateId);
                    }
                    if (!candidateModel.CommitteeProcessed)
                    {
                        string committeeMessage = AzureUtilities.MakeCandidateQueueMessage(candidateModel.CandidateId, state);
                        await committeeQueue.SendMessageAsync(committeeMessage);
                        log.LogInformation("{1} missing committee information", candidateModel.CandidateId);
                    }
                    if (!candidateModel.FinanceTotalProcessed)
                    {
                        string financeMessage = AzureUtilities.MakeCandidateQueueMessage(candidateModel.CandidateId, state);
                        await financeQueue.SendMessageAsync(financeMessage);
                        log.LogInformation("{1} missing finance information", candidateModel.CandidateId);
                    }
                    log.LogInformation("Completed MissingCandidateData for {1}", state);
                }
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            

            
        }
    }
}
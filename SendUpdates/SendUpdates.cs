using System;
using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.Storage.Queues;
using System.Linq;
using MDWatch.Model;
using MDWatch.Utilities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Azure.Identity;
using Azure.Data.Tables.Models;
using SharedComponents.Models;

namespace MDWatch
{
    public class SendUpdates
    {
        private AsyncPageable<TableItem> GetAllSolutionTables()
        {



            
            if (String.Equals(General.GetBuildEnv(), "Debug"))
            {
                var tableServiceClient = new TableServiceClient("UseDevelopmentStorage=true");
                
                return tableServiceClient.QueryAsync($"TableName gt '@'");
            }
            else
            {
                var tableServiceClient = new TableServiceClient(new Uri("https://stcandidatewatchdata01.table.core.windows.net"), new DefaultAzureCredential());
                return tableServiceClient.QueryAsync($"TableName gt '@'");
            }

        }
        [FunctionName("SendUpdates")]
        
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)

        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            //get all tables in solution, interate through them and generate queue messages
            AsyncPageable<TableItem> allTablesQuery = GetAllSolutionTables();



            await foreach (var table in allTablesQuery)
            {
                TableClient updateLogClient = AzureUtilities.GetTableClient(General.EnvVars["table_update_log"].ToString());
                StateUpdateLog stateUpdateLog = new();
                stateUpdateLog.UpdateTime=DateTime.Now.ToUniversalTime();
                
                //since we are very limited on filtering using table service client filtering options, we get all tables in the solution, and check if the table is the updates table log, which we will ignore
                if (!table.Name.Contains(General.EnvVars["table_update_log"].ToString()))
                {
                    string state = table.Name.Substring(0, 2);
                    stateUpdateLog.State = state;
                    //get entries from candidate partition that haven't been processed

                    TableClient tableClient = AzureUtilities.GetTableClient(state);
                    QueueClient committeeQueue = AzureUtilities.GetQueueClient(General.EnvVars["queue_committee"].ToString());
                    Pageable<TableEntity> committeeQuery = tableClient.Query<TableEntity>(filter: $"PartitionKey eq '{General.EnvVars["partition_candidate_status"].ToString()}' and CommitteeProcessed eq false");
                    if (committeeQuery.Count() > 0)
                    {
                        stateUpdateLog.CommitteesCount=committeeQuery.Count();
                        log.LogInformation("Found {1} candidates in {2} missing committee information: ", committeeQuery.Count(),state);
                        //write messages to queue for look up later, we only need the candidate ID to perform the lookup from the FEC API
                        foreach (var row in committeeQuery)
                        {
                            object candidateID = new object();
                            row.TryGetValue(Utilities.General.GetMemberName((Candidate c) => c.CandidateId), out candidateID);
                            await committeeQueue.SendMessageAsync(AzureUtilities.MakeCandidateQueueMessage(candidateID.ToString(), state));
                        }


                    }
                    QueueClient financeTotalsQueue = AzureUtilities.GetQueueClient(General.EnvVars["queue_finance_totals"].ToString());
                    Pageable<TableEntity> financeTotalsQuery = tableClient.Query<TableEntity>(filter: $"PartitionKey eq '{General.EnvVars["partition_candidate_status"].ToString()}' and FinanceTotalProcessed eq false");
                    if (financeTotalsQuery.Count() > 0)
                    {
                        stateUpdateLog.FinanceTotalsCount = financeTotalsQuery.Count();
                        log.LogInformation("Found {1} candidates missing Financial Total (aggregate) information: ", financeTotalsQuery.Count());
                        //write messages to queue for look up later, we only need the candidate ID to perform the lookup from the FEC API
                        foreach (var row in financeTotalsQuery)
                        {
                            object candidateID = new object();
                            row.TryGetValue(Utilities.General.GetMemberName((Candidate c) => c.CandidateId), out candidateID);

                            await financeTotalsQueue.SendMessageAsync(AzureUtilities.MakeCandidateQueueMessage(candidateID.ToString(), state));
                        }


                    }

                    QueueClient scheduleBQueue = AzureUtilities.GetQueueClient(General.EnvVars["queue_scheduleb_candidate"].ToString());
                    Pageable<TableEntity> scheduleBQuery = tableClient.Query<TableEntity>(filter: $"PartitionKey eq '{General.EnvVars["partition_candidate_status"].ToString()}' and ScheduleBProcessed eq false");
                    if (scheduleBQuery.Count() > 0)
                    {
                        stateUpdateLog.ScheduleBCount = scheduleBQuery.Count();
                        log.LogInformation("Found {1} candidates missing ScheduleB information: ", scheduleBQuery.Count());
                        //write messages to queue for look up later, we only need the candidate ID to perform the lookup from the FEC API
                        foreach (var row in scheduleBQuery)
                        {
                            object candidateID = new object();
                            row.TryGetValue(Utilities.General.GetMemberName((Candidate c) => c.CandidateId), out candidateID);
                            await scheduleBQueue.SendMessageAsync(AzureUtilities.MakeCandidateQueueMessage(candidateID.ToString(), state));
                        }


                    }
                    //write updatelog entry with calculted counts and run time
                    TableEntity updateLogEntity = stateUpdateLog.ModelToTableEntity(updateLogClient, General.EnvVars["partition_update_log"].ToString(), Guid.NewGuid().ToString());
                    await updateLogClient.AddEntityAsync(updateLogEntity);
                }
                
            }
            
            return new OkResult();
        }
    }
}

using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MDWatch.SolutionClients;
using MDWatch.Model;
using MDWatch.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace MDWatch
{
    public class CandidateFinance
    {
        private static string apiKey { get => General.GetFECAPIKey(); }
        

        [FunctionName("FinanceTotals")]
        public async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            //process candidate IDs by looking up candidate ID from queue message using FEC API, write data to table storage
            QueueClient queueClient = AzureUtilities.GetQueueClient(General.EnvVars["queue_finance_totals"].ToString());
            QueueMessage[] candidateIDs = await queueClient.ReceiveMessagesAsync(32);
            
            

            
            foreach (var candidate in candidateIDs)
            {
                CandidateQueueMessage queueMessage = AzureUtilities.ParseCandidateQueueMessage(candidate.Body.ToString());
                TableClient tableClient = AzureUtilities.GetTableClient(queueMessage.State);
                

                CandidateFinanceClient finance = new CandidateFinanceClient(apiKey);
                finance.SetQuery(new FECQueryParms { CandidateId = queueMessage.CandidateId });

                log.LogInformation("Getting aggregate financial information for candidate: {1}", queueMessage.CandidateId);

                try
                {
                    await finance.SubmitAsync();
                    var cycleTotals = from cycle in finance.Contributions where cycle.CandidateId.Contains(queueMessage.CandidateId) select cycle;
                    
                    var nonIndividualContributions = from c in finance.Contributions where c.CandidateId.Contains(queueMessage.CandidateId) select c.OtherPoliticalCommitteeContributions;
                    var individualContributions = from c in finance.Contributions where c.CandidateId.Contains(queueMessage.CandidateId) select c.IndividualItemizedContributions;
                    CandidateFinanceOverview overview = new()
                    {
                        CandidateId = queueMessage.CandidateId,
                        TotalIndividualContributions = (decimal)individualContributions.Sum(),
                        TotalNonIndividualContributions = (decimal)nonIndividualContributions.Sum()
                    };
                    
                    try
                    {
                        TableEntity overviewEntity = overview.ModelToTableEntity(tableClient, General.EnvVars["partition_finance_overview"].ToString(), overview.CandidateId);
                        
                        await tableClient.AddEntityAsync(overviewEntity);

                        foreach (var cycle in cycleTotals)
                        {
                         
                 
                            TableEntity cycleEntity = cycle.ModelToTableEntity(tableClient, General.EnvVars["partition_finance_totals"].ToString(), Guid.NewGuid().ToString());
                            await tableClient.AddEntityAsync(cycleEntity);
                        }
                        TableEntity entity = await tableClient.GetEntityAsync<TableEntity>(General.EnvVars["partition_candidate_status"].ToString(), queueMessage.CandidateId);
                        entity[Utilities.General.GetMemberName((CandidateStatus c) => c.FinanceTotalProcessed)] = true;
                        await tableClient.UpdateEntityAsync(entity, entity.ETag);
                        await queueClient.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
                    }
                    catch (Exception ex)
                    {
                        log.LogInformation(ex.ToString());
                        log.LogInformation("problem writing FinanceTotals to table storage for {1}", queueMessage.CandidateId);
                    }
                }
                catch (Exception ex)
                {
                    log.LogInformation(ex.ToString());
                    log.LogInformation("problem retrieving aggregate Financial information "); //TODO fix this, should reference the specific candidate
                }
            }
        }
    }
}
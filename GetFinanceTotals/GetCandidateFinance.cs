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
            QueueClient queueClient = new QueueClient("UseDevelopmentStorage=true", "financetotalsprocess");
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            QueueMessage[] candidateIDs = await queueClient.ReceiveMessagesAsync(32);
            CandidateFinanceClient finance = new CandidateFinanceClient(apiKey);
            //process candidate IDs by looking up candidate ID from queue message using FEC API, write data to table storage

            
            foreach (var candidate in candidateIDs)
            {
                finance.SetQuery(new FECQueryParms { CandidateId = candidate.Body.ToString() });

                log.LogInformation("Getting aggregate financial information for candidate: {1}", candidate.Body.ToString());

                try
                {
                    await finance.SubmitAsync();
                    var cycleTotals = from cycle in finance.Contributions where cycle.CandidateId.Contains(candidate.Body.ToString()) select cycle;
                    
                    var nonIndividualContributions = from c in finance.Contributions where c.CandidateId.Contains(candidate.Body.ToString()) select c.OtherPoliticalCommitteeContributions;
                    var individualContributions = from c in finance.Contributions where c.CandidateId.Contains(candidate.Body.ToString()) select c.IndividualItemizedContributions;
                    CandidateFinanceOverview overview = new()
                    {
                        CandidateId = candidate.Body.ToString(),
                        TotalIndividualContributions = (decimal)individualContributions.Sum(),
                        TotalNonIndividualContributions = (decimal)nonIndividualContributions.Sum()
                    };
                    
                    try
                    {
                        TableEntity overviewEntity = overview.ModelToTableEntity(tableClient, "FinanceOverview", overview.CandidateId);
                        await tableClient.AddEntityAsync(overviewEntity);

                        foreach (var cycle in cycleTotals)
                        {
                         
                 
                            TableEntity cycleEntity = cycle.ModelToTableEntity(tableClient, "FinanceTotals", Guid.NewGuid().ToString());
                            await tableClient.AddEntityAsync(cycleEntity);
                        }
                        TableEntity entity = await tableClient.GetEntityAsync<TableEntity>("CandidateStatus", candidate.Body.ToString());
                        entity[Utilities.General.GetMemberName((CandidateStatus c) => c.FinanceTotalProcessed)] = true;
                        await tableClient.UpdateEntityAsync(entity, entity.ETag);
                        await queueClient.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
                    }
                    catch (Exception ex)
                    {
                        log.LogInformation(ex.ToString());
                        log.LogInformation("problem writing FinanceTotals to table storage for {1}", candidate.Body.ToString());
                    }
                }
                catch (Exception ex)
                {
                    log.LogInformation(ex.ToString());
                    log.LogInformation("problem retrieving aggregate Financial information "); //todo fix this, should reference the specific candidate
                }
            }
        }
    }
}
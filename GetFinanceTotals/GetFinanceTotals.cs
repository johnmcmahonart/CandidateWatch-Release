using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Collections;
namespace FECIngest
{
    public class FinanceTotals
    {
        private const string apiKey = "xT2E5C0eUKvhVY74ylbGf4NWXz57XlxTkWV9pOwu";
        [FunctionName("FinanceTotals")]
        
        public async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            QueueClient queueClient = new QueueClient("UseDevelopmentStorage=true", "financetotalsprocess");
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            QueueMessage[] candidateIDs = await queueClient.ReceiveMessagesAsync(32);
            CandidateFinanceTotalsSearch financeTotals = new CandidateFinanceTotalsSearch(apiKey);
            //process candidate IDs by looking up candidate ID from queue message using FEC API, write data to table storage

            foreach (var candidate in candidateIDs)
            {
                financeTotals.SetCandidate(candidate.Body.ToString());

                log.LogInformation("Getting aggregate financial information for candidate: {1}", candidate.Body.ToString());
                bool result = await SharedComponents.PollyPolicy.GetDefault.ExecuteAsync(() => financeTotals.Submit());
                if (!result)
                {
                    log.LogInformation("problem retrieving CandidateIds from queue for processing");
                }
                else
                {
                    TableEntity entity = await tableClient.GetEntityAsync<TableEntity>("Candidate", candidate.Body.ToString());
                    entity["FinanceTotalProcessed"] = true;
                    await tableClient.UpdateEntityAsync(entity, entity.ETag);
                    await queueClient.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
                    
                    
                        //dates written to azure table storage must be UTC
                        var fixedItem = financeTotals.FinanceTotals[financeTotals.FinanceTotals.Count-1].AddUTC();
                        TableEntity fixedEntity = fixedItem.ToTable(tableClient, "FinanceTotals", Guid.NewGuid().ToString());
                        var errorState = await tableClient.AddEntityAsync(fixedEntity);


                        if (errorState.IsError) //schedule candidate to be processed later
                        {
                            //todo handle duplicate committee's during reprocessing
                            log.LogInformation("Problem writing finance total to storage for {1}", candidate.Body.ToString());
                            TableEntity failed = await tableClient.GetEntityAsync<TableEntity>("Candidate", candidate.Body.ToString());
                            entity["FinanceTotal"] = false;
                            await tableClient.UpdateEntityAsync(failed, failed.ETag);
                        }

                    

                }
            }
        }
    }
}
        
        
    


using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FECIngest
{
    public class FinanceTotals
    {
        private const string apiKey = "xT2E5C0eUKvhVY74ylbGf4NWXz57XlxTkWV9pOwu";

        [FunctionName("FinanceTotals")]
        public async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            QueueClient queueClient = new QueueClient("UseDevelopmentStorage=true", "financetotalsprocess");
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            QueueMessage[] candidateIDs = await queueClient.ReceiveMessagesAsync(32);
            CandidateFinanceTotals financeTotals = new CandidateFinanceTotals(apiKey);
            //process candidate IDs by looking up candidate ID from queue message using FEC API, write data to table storage

            foreach (var candidate in candidateIDs)
            {
                financeTotals.SetQuery(new Dictionary<string, string>()
                {
                    {
                     "candidateId", candidate.Body.ToString()
                    } }
                ); 

                log.LogInformation("Getting aggregate financial information for candidate: {1}", candidate.Body.ToString());
                bool result = await SharedComponents.PollyPolicy.GetDefault.ExecuteAsync(() => financeTotals.Submit());
                if (!result)
                {
                    log.LogInformation("problem retrieving CandidateIds from queue for processing");
                }
                else
                {
                    var cycleTotals = from cycle in financeTotals.Contributions where cycle.CandidateId.Contains(candidate.Body.ToString()) select cycle;
                    
                    try
                    {
                        foreach (var cycle in cycleTotals)
                        {
                            //dates written to azure table storage must be UTC
                            var fixedItem = cycle.AddUTC();
                            TableEntity fixedEntity = fixedItem.ToTable(tableClient, "FinanceTotals", Guid.NewGuid().ToString());
                            await tableClient.AddEntityAsync(fixedEntity);
                            
                            
                        }
                        TableEntity entity = await tableClient.GetEntityAsync<TableEntity>("Candidate", candidate.Body.ToString());
                        entity["FinanceTotalProcessed"] = true;
                        await tableClient.UpdateEntityAsync(entity, entity.ETag);
                        await queueClient.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
                    }
                    catch
                    {
                        log.LogInformation("problem writing FinanceTotals to table storage for {1}", candidate.Body.ToString());
                    }
                }
            }
        }
    }
}

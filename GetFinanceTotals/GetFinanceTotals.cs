using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using FECIngest.SolutionClients;
using FECIngest.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using FECIngest;

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
            CandidateFinanceTotalsClient financeTotals = new CandidateFinanceTotalsClient(apiKey);
            //process candidate IDs by looking up candidate ID from queue message using FEC API, write data to table storage

            foreach (var candidate in candidateIDs)
            {
                financeTotals.SetQuery(new FECQueryParms { CandidateId = candidate.Body.ToString() });

                log.LogInformation("Getting aggregate financial information for candidate: {1}", candidate.Body.ToString());

                try
                {
                    await financeTotals.SubmitAsync();
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
                        entity[Utilities.GetMemberName((Candidate c) => c.FinanceTotalProcessed)] = true;
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
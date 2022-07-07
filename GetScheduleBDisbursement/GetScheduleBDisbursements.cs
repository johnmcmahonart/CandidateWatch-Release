using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage;
using Azure.Storage.Queues.Models;

namespace FECIngest
{
    public class GetScheduleBDisbursements
    {
        private const string apiKey = "xT2E5C0eUKvhVY74ylbGf4NWXz57XlxTkWV9pOwu";

        [FunctionName("GetScheduleBDisbursements")]
        public async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            QueueClient queueClient = new QueueClient("UseDevelopmentStorage=true", "schedulebprocess");
            
            QueueMessage[] candidateIDs = await queueClient.ReceiveMessagesAsync(32);
            ScheduleBDisbursement scheduleBDisbursement = new ScheduleBDisbursement(apiKey);
            
            foreach (var candidate in candidateIDs)
            {
                TableEntity candidateEntity = await tableClient.GetEntityAsync<TableEntity>("Candidate", candidate.Body.ToString());
                //check for empty json string
                if (!String.Equals(candidateEntity.GetString("PrincipalCommittees-json"), "[]"))
                {
                    dynamic principalCommittee = JsonConvert.DeserializeObject(candidateEntity.GetString("PrincipalCommittees-json"));

                    string committeeId = principalCommittee[0]["committee_id"];

                    scheduleBDisbursement.SetQuery(new FECQueryParmsModel { CommitteeId = committeeId });
                    log.LogInformation("Getting ScheduleB data for candidate: {1}", candidateEntity.RowKey);
                    try
                    {
                        await SharedComponents.PollyPolicy.GetDefault.ExecuteAsync(() => scheduleBDisbursement.Submit());
                        do
                        {
                            await SharedComponents.PollyPolicy.GetDefault.ExecuteAsync(() => scheduleBDisbursement.GetNextPage());
                        } while (!scheduleBDisbursement.GetNextPage().Result.IsLastPage);
                    }
                    catch (Exception ex)
                    {
                        log.LogError(ex.Message);
                        log.LogInformation("Problem retrieving all ScheduleB data for:{1}", candidateEntity.RowKey);
                    }

                    var committeeDisbursements = from d in scheduleBDisbursement.Disbursements where d.RecipientId.Contains(committeeId) select d;
                    foreach (var item in committeeDisbursements)
                    {
                        var fixedDisbursement = item.AddUTC();
                        TableEntity disbursementEntity = fixedDisbursement.ToTable(tableClient, "ScheduleB", Guid.NewGuid().ToString());
                        disbursementEntity.Add("CandidateId", candidate.Body.ToString());
                        await tableClient.AddEntityAsync(disbursementEntity);
                        
                        
                    }
                    TableEntity entity = await tableClient.GetEntityAsync<TableEntity>("Candidate", candidate.Body.ToString());
                    entity["ScheduleBProcessed"] = true;
                    await tableClient.UpdateEntityAsync(entity, entity.ETag);
                    await queueClient.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
                }
            }

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
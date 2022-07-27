using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using FECIngest.Model;
using FECIngest.SolutionClients;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FECIngest
{
    public class GetScheduleBDisbursementsOverview
    //this class retrieves basic ScheduleB data, such as total number of disbursements, and number of result pages
    //this is used to generate queue messages to be processed by downstream worker
    //this is necessary because some candidates have hundreds of pages of ScheduleB data that can't be retrieved quickly within API rate limit and non-durable function lifetime
    {
        private const string apiKey = "xT2E5C0eUKvhVY74ylbGf4NWXz57XlxTkWV9pOwu";

        [FunctionName("GetScheduleBDisbursementsOverview")]
        public async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            QueueClient scheduleBCandidateQueue = new QueueClient("UseDevelopmentStorage=true", "schedulebcandidateprocess");
            ScheduleBDisbursementClient scheduleBDisbursement = new ScheduleBDisbursementClient(apiKey);
            QueueMessage[] candidateIDs = await scheduleBCandidateQueue.ReceiveMessagesAsync(32);

            foreach (var candidate in candidateIDs)
            {
                if (candidate.Body.ToString()=="H6MD04183")
                {
                    log.LogInformation("found candidate");
                }
                TableEntity candidateEntity = await tableClient.GetEntityAsync<TableEntity>("Candidate", candidate.Body.ToString());
                //check for empty json string

                dynamic principalCommittee = JsonConvert.DeserializeObject(candidateEntity.GetString("PrincipalCommittees-json"));
                    if (principalCommittee.Count > 0)
                    {
                        string committeeId = principalCommittee[0]["committee_id"];

                        //check if candidate has ScheduleB disbursements, if so store overview data
                        var candidateOverview = await GenerateScheduleBOverview(log, tableClient, scheduleBCandidateQueue, scheduleBDisbursement, candidate, committeeId);
                        if (candidateOverview.TotalDisbursements > 0)
                        {
               

                            //write messages to queue for each page, for each candidate
                            await GenerateScheduleBDetailMessages(scheduleBCandidateQueue, candidateOverview, candidate, candidateEntity);
                        }
                        else //no scheduleBDisbursements, remove from queue
                        {
                            await MarkProcessedandDequeue(log, tableClient, scheduleBCandidateQueue, candidate);
                        }
                    }
                    else //if candidate does not have an associated committee, mark entity as processed and remove message from queue
                    {
                        await MarkProcessedandDequeue(log, tableClient, scheduleBCandidateQueue, candidate);
                    }
                
                
            }
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }

        private async Task<ScheduleBCandidateOverview> GenerateScheduleBOverview(ILogger log, TableClient tableClient, QueueClient scheduleBCandidateQueue, ScheduleBDisbursementClient scheduleBDisbursement, QueueMessage candidate, string committeeId)
        {
            //write overview data to table, for validation worker to use
            //get overview data for candidate by asking for first page of results
            scheduleBDisbursement.SetQuery(new FECQueryParms
            {
                CommitteeId = committeeId,
                PageIndex = 1
            });

            
            try
            {
                await scheduleBDisbursement.SubmitAsync();
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.ToString());
                log.LogInformation("Problem retrieving scheduleB information for candidate:{1}", candidate.Body.ToString());
            }
            log.LogInformation("Total result pages for candidate: {1}={2}", candidate.Body.ToString(), scheduleBDisbursement.TotalPages);
            log.LogInformation("Total disbursements for candidate: {1}={2}", candidate.Body.ToString(), scheduleBDisbursement.TotalDisbursementsforCandidate);
            ScheduleBCandidateOverview scheduleBCandidateOverview = new ScheduleBCandidateOverview
            {
                CandidateId = candidate.Body.ToString(),
                TotalDisbursements = scheduleBDisbursement.TotalDisbursementsforCandidate,
                TotalResultPages = scheduleBDisbursement.TotalPages,
                PrincipalCommitteeId = committeeId
            };
            TableEntity scheduleBOverviewEntity = scheduleBCandidateOverview.ToTable(tableClient, "ScheduleBOverview", scheduleBCandidateOverview.CandidateId);
            try
            {
                await tableClient.AddEntityAsync(scheduleBOverviewEntity);
                //await scheduleBCandidateQueue.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
                
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.ToString());
                log.LogInformation("Problem writing scheduleB overview data for candidate");
                
            }
            return scheduleBCandidateOverview;
        }

        private async Task GenerateScheduleBDetailMessages(QueueClient scheduleBCandidateQueue, ScheduleBCandidateOverview scheduleBCandidateOverview, QueueMessage candidate, TableEntity candidateEntity)
        {
            QueueClient scheduleBPagesQueue = new QueueClient("UseDevelopmentStorage=true", "schedulebpageprocess");
            dynamic principalCommittee = JsonConvert.DeserializeObject(candidateEntity.GetString("PrincipalCommittees-json"));
            string recipientId = principalCommittee[0]["committee_id"];
            for (int i = 1; i <= scheduleBCandidateOverview.TotalResultPages; i++)
            {
                await scheduleBPagesQueue.SendMessageAsync(candidate.Body.ToString() + "," + recipientId + "," + i);
            }
            await scheduleBCandidateQueue.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
        }

        private async Task MarkProcessedandDequeue(ILogger log, TableClient tableClient, QueueClient scheduleBCandidateQueue, QueueMessage candidate)
        {
            TableEntity entity = await tableClient.GetEntityAsync<TableEntity>("Candidate", candidate.Body.ToString());
            entity["ScheduleBProcessed"] = true;
            log.LogInformation("Candidate: {1} No ScheduleB disbursements, will not be processed", candidate.Body.ToString());
            await tableClient.UpdateEntityAsync(entity, entity.ETag);
            await scheduleBCandidateQueue.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
        }
    }
}
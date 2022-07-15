using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using FECIngest.SolutionClients;
using FECIngest.Model;



namespace FECIngest
{
    public class GetScheduleBDisbursementsOverview
    //this class retrieves basic ScheduleB data, such as total number of disbursements, and number of result pages
    //this is used to generate queue messages to be processed by downstream worker
    //this is necessary because some candidates have hundreds of pages of ScheduleB data that can't be retrived quickly within API rate limit and non-durable function lifetime
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
                TableEntity candidateEntity = await tableClient.GetEntityAsync<TableEntity>("Candidate", candidate.Body.ToString());

                //check for empty json string

                dynamic principalCommittee = JsonConvert.DeserializeObject(candidateEntity.GetString("PrincipalCommittees-json"));
                if (principalCommittee.Count > 0)
                {
                    string committeeId = principalCommittee[0]["committee_id"];

                    scheduleBDisbursement.SetQuery(new FECQueryParms
                    {
                        CommitteeId = committeeId,
                        PageIndex = 1
                    });

                    //get overview data for candidate by asking for first page of results
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
                    
                    //check if candidate has ScheduleB disbursements, if so store overview data
                    if (scheduleBDisbursement.TotalDisbursementsforCandidate > 0)
                    {
                        //write overview data to table, for validation worker to use
                        ScheduleBCandidateOverview scheduleBCandidateOverview = new ScheduleBCandidateOverview
                        {
                            CandidateId = candidate.Body.ToString(),
                            TotalDisbursements = scheduleBDisbursement.TotalDisbursementsforCandidate,
                            TotalResultPages = scheduleBDisbursement.TotalPages,
                            PrincipalCommitteeId = committeeId
                            
                        };
                        TableEntity scheduleBOverview = scheduleBCandidateOverview.ToTable(tableClient, "ScheduleBOverview", scheduleBCandidateOverview.CandidateId);
                        try
                        {
                            await tableClient.AddEntityAsync(scheduleBOverview);
                            await scheduleBCandidateQueue.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
                        }
                        catch (Exception ex)
                        {
                            log.LogInformation(ex.ToString());
                            log.LogInformation("Problem writing scheduleB overview data for candidate");
                        }

                        //write messages to queue for each page, for each candidate
                        QueueClient scheduleBPagesQueue = new QueueClient("UseDevelopmentStorage=true", "schedulebpageprocess");
                        for (int i = 1; i < scheduleBDisbursement.TotalPages + 1; i++)
                        {
                            await scheduleBPagesQueue.SendMessageAsync(candidate.Body.ToString() + "," +committeeId+","+ i);
                        }
                    }
                    else //no scheduleBDisbursements, remove from queue
                    {
                        TableEntity entity = await tableClient.GetEntityAsync<TableEntity>("Candidate", candidate.Body.ToString());
                        entity["ScheduleBProcessed"] = true;
                        log.LogInformation("Candidate: {1} No ScheduleB disbursements, will not be processed", candidate.Body.ToString());
                        await tableClient.UpdateEntityAsync(entity, entity.ETag);
                        await scheduleBCandidateQueue.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
                    }
                }
                else //if candidate does not have an associated committee, mark entity as processed and remove message from queue
                {
                    TableEntity entity = await tableClient.GetEntityAsync<TableEntity>("Candidate", candidate.Body.ToString());
                    entity["ScheduleBProcessed"] = true;
                    log.LogInformation("Candidate: {1} No associated committee found , will not be processed", candidate.Body.ToString());
                    await tableClient.UpdateEntityAsync(entity, entity.ETag);
                    await scheduleBCandidateQueue.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
                }
            }

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
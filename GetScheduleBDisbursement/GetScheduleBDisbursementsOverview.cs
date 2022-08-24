using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MDWatch.ScheduleBDisbursement;
using MDWatch.SolutionClients;
using MDWatch.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MDWatch
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
            QueueMessage[] candidateIds = await scheduleBCandidateQueue.ReceiveMessagesAsync(32);

            foreach (var candidate in candidateIds)
            {
                TableEntity candidateEntity = await tableClient.GetEntityAsync<TableEntity>("Candidate", candidate.Body.ToString());
                //check for empty json string

                dynamic principalCommittee = JsonConvert.DeserializeObject(candidateEntity.GetString("PrincipalCommittees-json"));

                //logic is this. If the candidate has a principal committee, check if there is overview data for them, if they don't we dequeue the message and mark the candidate as processed. If there is, detail data was already written
                //We create the overviewdata and then remove the from the queue so we don't redownload the detail data. If not check if the candidate has ScheduleB disbursements. If they do, write to storage, otherwise dequeue the message and mark as processed

                if (principalCommittee.Count > 0)
                {
                    string committeeId = principalCommittee[0]["committee_id"];

                    if (ScheduleBHelper.CommitteeExistsinOverview(tableClient, committeeId))
                    {
                        //create candidateOverview but don't generate detail messages since they already exist

                        var candidateOverview = await ScheduleBHelper.GenerateScheduleBOverviewAsync(log, tableClient, scheduleBCandidateQueue, scheduleBDisbursement, candidate, committeeId);
                        await scheduleBCandidateQueue.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
                    }
                    else
                    {
                        var candidateOverview = await ScheduleBHelper.GenerateScheduleBOverviewAsync(log, tableClient, scheduleBCandidateQueue, scheduleBDisbursement, candidate, committeeId);
                        if (candidateOverview.TotalDisbursements > 0)
                        {
                            await ScheduleBHelper.GenerateScheduleBDetailMessagesAsync(scheduleBCandidateQueue, candidateOverview, candidate, candidateEntity);
                        }
                        else //no scheduleBDisbursements, remove from queue
                        {
                            await ScheduleBHelper.MarkProcessedandDequeueAsync(log, tableClient, scheduleBCandidateQueue, candidate);
                        }
                    }
                }
                else //if candidate does not have an associated committee, mark entity as processed and remove message from queue
                {
                    await ScheduleBHelper.MarkProcessedandDequeueAsync(log, tableClient, scheduleBCandidateQueue, candidate);
                }
            }
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
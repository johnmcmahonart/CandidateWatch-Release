using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MDWatch;
using MDWatch.Model;
using MDWatch.SolutionClients;
using MDWatch.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MDWatch.ScheduleBDisbursement
{
    internal static class ScheduleBHelper
    {
        public static bool CommitteeExistsinOverview(TableClient tableClient, string committeeId)
        {

            Pageable<TableEntity> foundCandidates = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'ScheduleBOverview' and {Utilities.General.GetMemberName((ScheduleBCandidateOverview c) => c.PrincipalCommitteeId)} eq '{committeeId}'");
            
            if (foundCandidates.Any())
            {
                return true;
            }
            return false;
        }

        public static async Task<ScheduleBCandidateOverview> GenerateScheduleBOverviewAsync(ILogger log, TableClient tableClient, QueueClient scheduleBCandidateQueue, ScheduleBDisbursementClient scheduleBDisbursement, QueueMessage candidate, string committeeId)
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
            TableEntity scheduleBOverviewEntity = scheduleBCandidateOverview.ModelToTableEntity(tableClient, "ScheduleBOverview", scheduleBCandidateOverview.CandidateId);
            try
            {
                await tableClient.AddEntityAsync(scheduleBOverviewEntity);

            }
            catch (Exception ex)
            {
                log.LogInformation(ex.ToString());
                log.LogInformation("Problem writing scheduleB overview data for candidate");
            }
            return scheduleBCandidateOverview;
        }

        public static async Task GenerateScheduleBDetailMessagesAsync(QueueClient scheduleBCandidateQueue, ScheduleBCandidateOverview scheduleBCandidateOverview, QueueMessage candidate, TableEntity candidateEntity)
        {
            QueueClient scheduleBPagesQueue = new QueueClient("UseDevelopmentStorage=true", "schedulebpageprocess");
            dynamic principalCommittee = JsonConvert.DeserializeObject(candidateEntity.GetString("PrincipalCommitteesJson"));
            string recipientId = principalCommittee[0]["committee_id"];
            for (int i = 1; i <= scheduleBCandidateOverview.TotalResultPages; i++)
            {
                await scheduleBPagesQueue.SendMessageAsync(candidate.Body.ToString() + "," + recipientId + "," + i);
            }
            await scheduleBCandidateQueue.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
        }

        public static async Task MarkProcessedandDequeueAsync(ILogger log, TableClient tableClient, QueueClient scheduleBCandidateQueue, QueueMessage candidate)
        {
            TableEntity entity = await tableClient.GetEntityAsync<TableEntity>("CandidateStatus", candidate.Body.ToString());
            entity["ScheduleBProcessed"] = true;
            log.LogInformation("Candidate: {1} No ScheduleB disbursements, will not be processed", candidate.Body.ToString());
            await tableClient.UpdateEntityAsync(entity, entity.ETag);
            await scheduleBCandidateQueue.DeleteMessageAsync(candidate.MessageId, candidate.PopReceipt);
        }
    }

}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using FECIngest.Model;
using FECIngest.SolutionClients;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace FECIngest.ScheduleBDisbursement
{

    static internal class FindCandidateforCycle
    {
        //the same person can have multiple candidate ids, but the same committeeId for disbursements. In order to find which election cycle
        //is associated with a given candidate, we must find the candidate with the least number of cycles for a given cycle since it will be the most recent entry


        private static async Task<bool> HasCycleAsync(TableEntity entity, int findCycle)
        {
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            TableEntity foundCandidate = await tableClient.GetEntityAsync<TableEntity>("Candidate", (string)entity[Utilities.GetMemberName((ScheduleBCandidateOverview c) => c.PrincipalCommitteeId)]);
            dynamic principalCommittee = JsonConvert.DeserializeObject(foundCandidate.GetString("PrincipalCommittees-json"));

            foreach (var cycle in principalCommittee[0]["cycles"][0])


            {
                if (cycle == findCycle)
                {
                    return true;

                }
            }
            return false;
        }
        private static async Task<int> GetCycleCountAsync(TableEntity entity)
        {
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            TableEntity foundCandidate = await tableClient.GetEntityAsync<TableEntity>("Candidate", (string)entity[Utilities.GetMemberName((ScheduleBCandidateOverview c) => c.PrincipalCommitteeId)]);
            dynamic principalCommittee = JsonConvert.DeserializeObject(foundCandidate.GetString("PrincipalCommittees-json"));
            return principalCommittee[0]["cycles"][0].Count();

        }
        public static async Task<string> CommitteeSearchAsync(ScheduleBByRecipientID schedubleB, int cycle)
        {
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");

            //get candidates that match committeeId
            Pageable<TableEntity> candidatesWithCommittee = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'ScheduleBOverview' and {Utilities.GetMemberName((ScheduleBCandidateOverview c) => c.PrincipalCommitteeId)} eq '{schedubleB.CommitteeId}'");

            //find candidate with the fewest cycles from Candidate partition



            int leastCycles = await GetCycleCountAsync(candidatesWithCommittee.First());
            string candidateForCycle = candidatesWithCommittee.First().GetString("CandidateId");
            foreach (var candidate in candidatesWithCommittee)

            {
                if (await HasCycleAsync(candidate, cycle))
                {
                    if (await GetCycleCountAsync(candidate) < leastCycles)
                    {
                        leastCycles = await GetCycleCountAsync(candidatesWithCommittee.First());
                        candidateForCycle = candidate.GetString("CandidateId");
                    }
                }


            }
            return candidateForCycle;
        }


    }
}

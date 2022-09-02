using System;
using System.Collections.Generic;
using MDWatch.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using Azure;
using MDWatch.Utilities;
using System.Threading.Tasks;
using Polly;
using System.Collections.Concurrent;

namespace MDWatch
{
    public class BuildCandidatebyYearPartition
    {
        //builds partition of candidates grouped by year
        private const string apiKey = "xT2E5C0eUKvhVY74ylbGf4NWXz57XlxTkWV9pOwu";
        private const string _partitionKey = "CandidatebyYear";
        [FunctionName("BuildCandidatebyYearPartition")]
        
        //this function is used to optimize retrieval of candidates grouped by date for UI and repository
        public static async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            //clear table so data can be regenerated
            TablePurge.Purge(_partitionKey);
            List < Candidate > allCandidatesModel = new();

            
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            
            AsyncPageable<TableEntity> candidates = tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq 'Candidate'");
            
            //convert table entity to model
            await foreach (var candidate in candidates)
            {
                allCandidatesModel.Add(candidate.TableEntityToModel<Candidate>());
            }

            //sort model
             CandidatebyYear sortedCandidates = CandidateSort.Year((IEnumerable<Candidate>)allCandidatesModel);
            
            //write to table storage
                 
            await tableClient.AddEntityAsync<TableEntity>( sortedCandidates.ModelToTableEntity(tableClient, _partitionKey, Guid.NewGuid().ToString()));
            
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        
        }
    }
}

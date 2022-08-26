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
        public static async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            //clear table so data can be regenerated
            TablePurge.Purge(_partitionKey);


            
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            List<Candidate> outList = new List<Candidate>();
            AsyncPageable<TableEntity> candidates = tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq 'Candidate'");
            await foreach (var candidate in candidates)
            {
                var candidateModel = candidate.TableEntityToModel<Candidate>();
                foreach (var item in candidateModel.ElectionYears)
                {
                    if (outModel.year.ContainsKey(item))
                    {
                        outModel.year[item].Add(candidateModel.CandidateId);
                    }
                        else //it hasn't so initialize a new list for the year
                        {
                            outModel.year.Add(item, new List<string> { candidateModel.CandidateId });
                        }
                    
                    
                }
            }
            //write to table storage
            foreach (var item in outModel.year)
            {
                await tableClient.AddEntityAsync<TableEntity>( item.ModelToTableEntity(tableClient, _partitionKey, item.Key.ToString()));
            }
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        
        }
    }
}

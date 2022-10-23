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
using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace MDWatch
{
    public class BuildCandidatebyYearPartition
    {
        //builds partition of candidates grouped by year
        private static string apiKey { get => General.GetFECAPIKey(); }
        
        private const string _partitionKey = "CandidatebyYear";
        [FunctionName("BuildCandidatebyYearPartition")]

        //this function is used to optimize retrieval of candidates grouped by date for UI and repository
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string state = req.Query["state"];
            //clear table so data can be regenerated
            TablePurge.Purge(_partitionKey, state);
            List < Candidate > allCandidatesModel = new();

            
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            
            AsyncPageable<TableEntity> candidates = tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq 'Candidate'");
            
            //convert table entity to model
            await foreach (var candidate in candidates)
            {
                allCandidatesModel.Add(candidate.TableEntityToModel<Candidate>());
            }

            //sort model
             IEnumerable<CandidatebyYear> sortedCandidates = CandidateSort.Year((IEnumerable<Candidate>)allCandidatesModel);

            //write to table storage

            foreach (var year in sortedCandidates)
            {
                await tableClient.AddEntityAsync<TableEntity>(year.ModelToTableEntity(tableClient, _partitionKey, year.Year.ToString()));
            }
            
            
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            return new OkResult();
        }
    }
}

using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using FECIngest.SolutionClients;
using FECIngest.Model;
using FECIngest.Utilities;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Azure;
using FECIngest;
using System.Linq;
namespace Tests
{
    public class FromTable
    {
        [FunctionName("FromTable")]
        public async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            Pageable<TableEntity> candidateTable = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'Candidate'");
            foreach (var entity in candidateTable)
            {
                Candidate candidate = entity.TableEntityToModel<Candidate>();
                log.LogInformation("Candidate is {1}",candidate.CandidateId);
            }
            
        }
    }
}

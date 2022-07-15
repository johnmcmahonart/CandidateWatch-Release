using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using FECIngest;
using FECIngest.Model;
using FECIngest.SolutionClients;

namespace GetScheduleBDisbursement
{
    public class GetScheduleBDisbursementsDetail
    {
        //this class retrieves detailed (aggregate) ScheduleB data, such as amount of disbursements by a committee towards the candidate total number of disbursements, and number of result pages
        private const string apiKey = "xT2E5C0eUKvhVY74ylbGf4NWXz57XlxTkWV9pOwu";

        [FunctionName("GetScheduleBDisbursementsDetail")]
    
    
        public async Task Run([TimerTrigger("0 */500 * * * *")]TimerInfo myTimer, ILogger log)
        {
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            QueueClient scheduleBPageProcess = new QueueClient("UseDevelopmentStorage=true", "schedulebpageprocess");
            ScheduleBDisbursementClient scheduleBDisbursement = new ScheduleBDisbursementClient(apiKey);
            QueueMessage[] scheduleBPage = await scheduleBPageProcess.ReceiveMessagesAsync(32);

//get committee page detail, write to storage, remove from queue
            foreach (var page in scheduleBPage)
            {
                string[] scheduleBToken = page.Body.ToString().Split(',');
                scheduleBDisbursement.SetQuery(new FECQueryParms
                {
                    CommitteeId = scheduleBToken[1],
                    PageIndex = Int32.Parse(scheduleBToken[2])
                });
                try
                {
                    await scheduleBDisbursement.SubmitAsync();
                }
                
            catch(Exception ex)
                {
                    log.LogInformation(ex.ToString());
                    log.LogInformation("Problem retrieving scheduleB details information for committee:{1} on page:{2}", scheduleBToken[1], scheduleBToken[2]);
                }
                foreach (var item in scheduleBDisbursement.Disbursements)
                {
                    var fixedItem = item.AddUTC();
                    var scheduleBDetailEntity = fixedItem.ToTable(tableClient, "ScheduleBDetail", Guid.NewGuid().ToString());
                    await tableClient.AddEntityAsync(scheduleBDetailEntity);
                }
                await scheduleBPageProcess.DeleteMessageAsync(page.MessageId, page.PopReceipt);
            }
            
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}

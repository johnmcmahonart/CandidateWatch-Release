using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using FECIngest;
using FECIngest.Model;
using FECIngest.SolutionClients;
using FECIngest.Utilities;
using FECIngest.ScheduleBDisbursement;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FECIngest
{
    public class GetScheduleBDisbursementsDetail
    {
        //this class retrieves detailed (aggregate) ScheduleB data, such as amount of disbursements by a committee towards the candidate total number of disbursements, and number of result pages
        private const string apiKey = "xT2E5C0eUKvhVY74ylbGf4NWXz57XlxTkWV9pOwu";

        [FunctionName("GetScheduleBDisbursementsDetail")]
        public async Task Run([TimerTrigger("0 */3 * * * *")] TimerInfo myTimer, ILogger log)
        {
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
            QueueClient scheduleBPageProcess = new QueueClient("UseDevelopmentStorage=true", "schedulebpageprocess");
            ScheduleBDisbursementClient scheduleBDisbursement = new ScheduleBDisbursementClient(apiKey);
            QueueMessage[] scheduleBPages = await scheduleBPageProcess.ReceiveMessagesAsync(32);

            //get committee page detail, write to storage, remove from queue
            foreach (var page in scheduleBPages)
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
                    foreach (var item in scheduleBDisbursement.Disbursements)
                    {

                        var fixedItem = item.AddUTC();
                        var scheduleBDetailEntity = fixedItem.ModelToTableEntity(tableClient, "ScheduleBDetail", Guid.NewGuid().ToString());
                        await tableClient.AddEntityAsync(scheduleBDetailEntity);

                    }
                    log.LogInformation("Added page {1} for {2} to storage", scheduleBToken[2], scheduleBToken[1]);
                    
                    await scheduleBPageProcess.DeleteMessageAsync(page.MessageId, page.PopReceipt);
                }
                catch (Exception ex)
                {
                    log.LogInformation(ex.ToString());
                    log.LogInformation("Problem retrieving scheduleB details information for committee:{1} on page:{2}", scheduleBToken[1], scheduleBToken[2]);
                }
                
                
            }

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
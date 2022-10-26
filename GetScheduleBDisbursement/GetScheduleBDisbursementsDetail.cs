using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MDWatch;
using MDWatch.Model;
using MDWatch.SolutionClients;
using MDWatch.Utilities;
using MDWatch.ScheduleBDisbursement;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using SharedComponents.Models;

namespace MDWatch
{
    public class GetScheduleBDisbursementsDetail
    {
        //this class retrieves detailed (aggregate) ScheduleB data, such as amount of disbursements by a committee towards the candidate total number of disbursements, and number of result pages
        private static string apiKey { get => General.GetFECAPIKey(); }
        

        [FunctionName("GetScheduleBDisbursementsDetail")]
        public async Task Run([TimerTrigger("0 */3 * * * *")] TimerInfo myTimer, ILogger log)
        {
            
            QueueClient scheduleBPageProcess = AzureUtilities.GetQueueClient(General.EnvVars["queue_scheduleb_page"].ToString());
            QueueMessage[] scheduleBPages = await scheduleBPageProcess.ReceiveMessagesAsync(32);
            
            ScheduleBDisbursementClient scheduleBDisbursement = new ScheduleBDisbursementClient(apiKey);
            

            //get committee page detail, write to storage, remove from queue
            foreach (var page in scheduleBPages)
            {
                ScheduleBQueueMessage queueMessage = AzureUtilities.ParseScheduleBQueueMessage(page.Body.ToString());
                TableClient tableClient = AzureUtilities.GetTableClient(queueMessage.State);
                
                
                
                scheduleBDisbursement.SetQuery(new FECQueryParms
                {
                    CommitteeId = queueMessage.CommitteeId,
                    PageIndex = queueMessage.PageIndex
                });
                try
                {
                    await scheduleBDisbursement.SubmitAsync();
                    foreach (var item in scheduleBDisbursement.Disbursements)
                    {

                        var fixedItem = item.AddUTC();
                        var scheduleBDetailEntity = fixedItem.ModelToTableEntity(tableClient, General.EnvVars["partition_scheduleB_detail"].ToString(), Guid.NewGuid().ToString());
                        await tableClient.AddEntityAsync(scheduleBDetailEntity);

                    }
                    log.LogInformation("Added page {1} for {2} to storage", queueMessage.PageIndex, queueMessage.State);
                    
                    await scheduleBPageProcess.DeleteMessageAsync(page.MessageId, page.PopReceipt);
                }
                catch (Exception ex)
                {
                    log.LogInformation(ex.ToString());
                    log.LogInformation("Problem retrieving scheduleB details information for committee:{1} on page:{2}", queueMessage.CommitteeId, queueMessage.PageIndex);
                }
                
                
            }

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
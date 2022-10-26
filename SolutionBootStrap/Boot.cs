using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using MDWatch.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MDWatch
{
    //this function populates states into the statecandidatequeue which starts the workflow to populate the solution with data
    public static class Boot
    {
        [FunctionName("Boot")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string responseMessage = "";
            log.LogInformation("Booting Solution...");

            try
            {
                string statesPath = "states.json";
                string statesJson = await new StreamReader(statesPath).ReadToEndAsync();
                dynamic states = JsonConvert.DeserializeObject(statesJson);
                QueueClient stateQueueClient = AzureUtilities.GetQueueClient(General.EnvVars["queue_state_candidate"].ToString());
                QueueClient uiQueueClient = AzureUtilities.GetQueueClient(General.EnvVars["queue_ui_build"].ToString());
                foreach (string state in states)
                {
                    await stateQueueClient.SendMessageAsync(state);
                    await uiQueueClient.SendMessageAsync(state);
                }
                log.LogInformation("Application Boot completed Successfully. Please be patient while data is loaded into the solution. Run Ui build via http trigger when all data is loaded into solution.");
                responseMessage = "Application Boot completed Successfully. Please be patient while data is loaded into the solution. Run Ui build via http trigger when all data is loaded into solution.";
            }
            catch
            {
                log.LogInformation("APPLICATION BOOT FAILED: VALIDATE DATA STATE BEFORE TRYING AGAIN");
                responseMessage = "APPLICATION BOOT FAILED: VALIDATE DATA STATE BEFORE TRYING AGAIN";
            }


            log.LogInformation("Application Boot completed Successfully. Please be patient while data is loaded into the solution. Run Ui build via http trigger when all data is loaded into solution.");
            

            return new OkObjectResult(responseMessage);
        }
    }
}
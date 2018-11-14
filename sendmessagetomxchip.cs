using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using System.Text;

namespace Iot.MxChip
{
    public static class sendmessagetomxchip
    {
        [FunctionName("sendmessagetomxchip")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"Starting function {requestBody}");
            var connectionString = "<iothubdeviceconnectionstring>";
            var serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            log.LogInformation("Created servie client");
            var message = new Message(Encoding.ASCII.GetBytes(requestBody));
            log.LogInformation("sending message");
            try{
                await serviceClient.SendAsync("<devicename>", message);
            }catch(Exception ex){
                log.LogInformation(ex.Message);
                throw;
            }
            log.LogInformation("sent message to device");
            // log.LogInformation("C# HTTP trigger function processed a request.");

            // string name = req.Query["name"];

            
            // dynamic data = JsonConvert.DeserializeObject(requestBody);
            // name = name ?? data?.name;

            return (ActionResult)new OkObjectResult($"OK");
        }
    }
}

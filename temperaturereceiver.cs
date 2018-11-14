using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MxChip.Function
{
    public static class temperaturereceiver
    {
        [FunctionName("temperaturereceiver")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("temperature receiver function.");

            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var temperature = data?.temperature;
            var tempdecimal = decimal.Parse(temperature.ToString());
            var fareinheit = ((tempdecimal *9)/5) + 32;
            log.LogInformation($"Temperature is {temperature}");
            log.LogInformation($"Temperature in fareinheit is {fareinheit}");
            return  (ActionResult)new OkObjectResult($"{fareinheit}");
        }
    }
}

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
using Newtonsoft.Json;
using System.Text;
using System;
namespace Company.Function
{
    public static class HttpTrigger1
    {
        [FunctionName("HttpTrigger1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            string deviceKey = "test-vishnu-1";
            ServiceClient serviceClient = ServiceClient.CreateFromConnectionString("HostName=test-vishnu.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=BY0ePJJyVuNonD9bPYByWGZ3RPQenAKLyCFmNOxsXDc=");
            string deviceId = "+Nfnhsg5IK2d+2zPmMN4mhLLcHUWcygMuEamOFFNmr8=";
            string iotHubHostName = "test-vishnu.azure-devices.net";
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            double currentTemperature = 20 + 3 * 15;
            double currentHumidity = 60 + 4 * 20;

            var telemetryDataPoint = new
            {
                messageId = "123",
                deviceId = deviceId,
                temperature = currentTemperature,
                humidity = currentHumidity
            };
            string messageString = JsonConvert.SerializeObject(telemetryDataPoint);
            Message message = new Message(Encoding.ASCII.GetBytes(messageString));
            message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");

            await serviceClient.SendAsync("test-vishnu-1", message);
            Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

            await Task.Delay(1000);
            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}

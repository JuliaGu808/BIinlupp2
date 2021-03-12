using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FunctionAppInlupp2.Models;
using System;

namespace FunctionAppInlupp2
{
    public static class SaveToCosmos
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("SaveToCosmos")]
        public static void Run(
            [IoTHubTrigger("messages/events", Connection = "iothub-ehub-iothubdemo-8676784-bb8643f378", ConsumerGroup = "cosmos")] EventData message,
            [CosmosDB(
                databaseName:"juliacosmosdb2",
                collectionName:"dhtmeasurements",
                ConnectionStringSetting ="CosmosDb",
                CreateIfNotExists =true
            )] out dynamic cosmosdb,
            ILogger log)
        {
            //log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");
            var msg = JsonConvert.DeserializeObject<Messages>(Encoding.UTF8.GetString(message.Body.Array));
            
            CosmosMsg cosmosMsg = new CosmosMsg();
            cosmosMsg.deviceId = message.SystemProperties["iothub-connection-device-id"].ToString();
            
            cosmosMsg.location = message.Properties["location"].ToString();
            
            cosmosMsg.latitude = message.Properties["latitude"].ToString();
            log.LogInformation($"{cosmosMsg.latitude} ---- ");
            cosmosMsg.longitude = message.Properties["longitude"].ToString();
            
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(msg.ts);
            cosmosMsg.utc = dateTimeOffset.UtcDateTime;

            var json = JsonConvert.SerializeObject(cosmosMsg);
            log.LogInformation($"{json}...");
            cosmosdb = json;
        }
    }
}
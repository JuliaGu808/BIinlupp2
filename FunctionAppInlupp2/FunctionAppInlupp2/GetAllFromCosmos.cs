using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FunctionAppInlupp2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionAppInlupp2
{
    public static class GetAllFromCosmos
    {
        [FunctionName("GetAllFromCosmos")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
             [CosmosDB(
                databaseName:"juliacosmosdb2",
                collectionName:"dhtmeasurements",
                ConnectionStringSetting ="CosmosDb",
                SqlQuery ="SELECT * FROM c"
            )]IEnumerable<CosmosMsg> cosmos,
            ILogger log)
        {
            

            return new OkObjectResult(cosmos);
        }
    }
}


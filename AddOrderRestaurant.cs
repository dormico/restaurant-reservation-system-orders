using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;

namespace Orders;
public static class AddOrderRestaurant
{
    [FunctionName("AddOrderRestaurant")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        [CosmosDB(
            databaseName: "Restaurants",
            collectionName: "Orders",
            ConnectionStringSetting = "CosmosDBConnectionString")]IAsyncCollector<dynamic> documentsOut,
        ILogger log)
    {
        log.LogInformation("AddOrderRestaurant HTTP trigger function processed a request.");

        string rId = req.Query["rid"];

        var orders = Array.Empty<string>();

        PartitionKey partKey = new PartitionKey(rId);

        if (!string.IsNullOrEmpty(rId))
        {
            await documentsOut.AddAsync(new
            {
                id = rId,
                orders = orders
            });
        }

        return new OkObjectResult("Got rId: " + rId);
    }
}


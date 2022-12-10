using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Orders;
public static class getRestaurantOrders
{
    [FunctionName("GetRestaurantOrders")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetRestaurantOrders/{id}")] HttpRequest req,
        [CosmosDB(
                databaseName: "Restaurants",
                collectionName: "Orders",
                ConnectionStringSetting = "CosmosDBConnectionString",
                Id = "{id}",
                PartitionKey = "{id}")] OrderRestaurants restaurant,
        ILogger log)
    {
        log.LogInformation("GetRestaurantOrders HTTP trigger function processed a request.");

        if (restaurant == null)
        {
            log.LogInformation($"Restaurant not found");
            return new NotFoundResult();
        }
        else
        {
            log.LogInformation($"Found restaurant with id: {restaurant.id}");          
            var response = new { Orders = restaurant.Orders };
            return new OkObjectResult(response);
        }
    }
}

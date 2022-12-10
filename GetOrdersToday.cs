using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Orders;

public static class GetOrdersToday
{
    [FunctionName("GetOrdersToday")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetOrdersToday/{id}")] HttpRequest req,
        [CosmosDB(
                databaseName: "Restaurants",
                collectionName: "Orders",
                ConnectionStringSetting = "CosmosDBConnectionString",
                Id = "{id}",
                PartitionKey = "{id}")] OrderRestaurants restaurant,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string date = req.Query["date"]; //Format should be YYYY-MM-DD
        log.LogInformation("Date: " + date);

        var ordersToday = new List<Order>(); 
        foreach (var item in restaurant.Orders)
        {
            if(item.Date == date){
                ordersToday.Add(item);
                log.LogInformation("New order added!");
            }
        }
        log.LogInformation("Got the following orders: ");
        foreach (var item in ordersToday)
        {
            log.LogInformation(item.GuestEmail + " ordered " + item.Orders.Length + " dishes.");
        }

        var result = new { Orders = ordersToday };        

        return new OkObjectResult(result);
    }
}


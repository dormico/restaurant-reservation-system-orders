using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Orders;


public class Constants
{
    public const string COSMOS_DB_DATABASE_NAME = "Restaurants";
    public const string COSMOS_DB_CONTAINER_NAME = "Orders";
}
public class AppendNewOrder
{
    private readonly ILogger _logger;
    private CosmosClient _cosmosClient;

    private Database _database;
    private Container _container;

    public AppendNewOrder(
        ILogger<AppendNewOrder> logger,
        CosmosClient cosmosClient
        )
    {
        _logger = logger;
        _cosmosClient = cosmosClient;

        _database = _cosmosClient.GetDatabase(Constants.COSMOS_DB_DATABASE_NAME);
        _container = _database.GetContainer(Constants.COSMOS_DB_CONTAINER_NAME);
    }

    [FunctionName(nameof(AppendNewOrder))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "AppendNewOrder")] HttpRequest req)
    {
        IActionResult returnValue = null;

        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var input = JsonConvert.DeserializeObject<Order>(requestBody);

            var newOrder = new Order
            {
                id = Guid.NewGuid().ToString(),
                restaurantId = input.restaurantId,
                GuestEmail = input.GuestEmail,
                Takeaway = input.Takeaway,
                Date = input.Date,
                Hour = input.Hour,
                Min = input.Min,
                Duration = input.Duration,
                Tables = input.Tables,
                Orders = input.Orders
            };

            var path = "/orders/-" ;
            _logger.LogInformation("Path: " + path);
            List<PatchOperation> patchOperations = new List<PatchOperation>()
                {
                    PatchOperation.Set(path: path , value: newOrder)
                };

            ItemResponse<dynamic> item = await _container.PatchItemAsync<dynamic>(
                id: newOrder.restaurantId,
                partitionKey: new PartitionKey(newOrder.restaurantId),
                patchOperations: patchOperations
                );

            _logger.LogInformation("Item inserted");
            _logger.LogInformation($"This query cost: {item.RequestCharge} RU/s");
            returnValue = new OkObjectResult(new { id = newOrder.id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Could not insert item. Exception thrown: {ex.Message}");
            returnValue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return returnValue;
    }
}

//using GIBDemo.Core.Helpers;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: FunctionsStartup(typeof(Orders.Startup))]
namespace Orders;
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddFilter(level => true);
        });

        var config = (IConfiguration)builder.Services.First(d => d.ServiceType == typeof(IConfiguration)).ImplementationInstance;

        var connStr = Environment.GetEnvironmentVariable("CosmosDBConnectionString");

        builder.Services.AddSingleton((s) =>
        {
            CosmosClientBuilder cosmosClientBuilder = new CosmosClientBuilder(connStr);

            return cosmosClientBuilder.WithConnectionModeDirect()
                .WithBulkExecution(true)
                .Build();
        });
    }
}
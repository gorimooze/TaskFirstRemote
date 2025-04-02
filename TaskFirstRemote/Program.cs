using System;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskFirstRemote.Core.Interfaces;
using TaskFirstRemote.Infrastructure.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddHttpClient();

        services.AddSingleton<IWeatherService, OpenWeatherService>();
        services.AddSingleton<ILogStorage, AzureTableLogStorage>();
        services.AddSingleton<IPayloadStorage, AzureBlobPayloadStorage>();

        services.AddSingleton<TableServiceClient>(_ =>
            new TableServiceClient("UseDevelopmentStorage=true"));

        services.AddSingleton(_ =>
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var blobClient = new BlobServiceClient(connectionString);
            var containerClient = blobClient.GetBlobContainerClient("weatherpayloads");
            containerClient.CreateIfNotExists();
            return containerClient;
        });
    })
    .Build();

host.Run();
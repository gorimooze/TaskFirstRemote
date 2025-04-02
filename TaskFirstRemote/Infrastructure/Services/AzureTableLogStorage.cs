using System;
using System.Collections.Generic;
using Azure.Data.Tables;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFirstRemote.Core.Interfaces;
using TaskFirstRemote.Core.Models;

namespace TaskFirstRemote.Infrastructure.Services
{
    public class AzureTableLogStorage : ILogStorage
    {
        private readonly TableClient _tableClient;

        public AzureTableLogStorage(TableServiceClient tableServiceClient)
        {
            _tableClient = tableServiceClient.GetTableClient("WeatherLogs");
            _tableClient.CreateIfNotExists();
        }

        public async Task SaveLogAsync(WeatherLog log)
        {
            var entity = new TableEntity(log.Timestamp.ToString("yyyy-MM-dd"), log.Id)
            {
                { "Timestamp", log.Timestamp },
                { "Status", log.Status },
                { "ErrorMessage", log.ErrorMessage ?? string.Empty },
                { "BlobName", log.BlobName ?? string.Empty }
            };
            await _tableClient.AddEntityAsync(entity);
        }

        public async Task<IEnumerable<WeatherLog>> GetLogsAsync(DateTime from, DateTime to)
        {
            var result = new List<WeatherLog>();

            var filter = $"Timestamp ge datetime'{from:o}' and Timestamp le datetime'{to:o}'";

            await foreach (var entity in _tableClient.QueryAsync<TableEntity>(filter))
            {
                result.Add(new WeatherLog
                {
                    Id = entity.RowKey,
                    Timestamp = entity.GetDateTime("Timestamp") ?? DateTime.MinValue,
                    Status = entity.GetString("Status") ?? "Unknown",
                    ErrorMessage = entity.GetString("ErrorMessage"),
                    BlobName = entity.GetString("BlobName")
                });
            }

            return result;
        }
    }
}

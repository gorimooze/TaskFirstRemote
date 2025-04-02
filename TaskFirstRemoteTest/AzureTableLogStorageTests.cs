using Azure;
using Azure.Data.Tables;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskFirstRemote.Core.Models;
using TaskFirstRemote.Infrastructure.Services;
using Xunit;

public class AzureTableLogStorageTests
{
    private readonly Mock<TableClient> _mockTableClient;
    private readonly AzureTableLogStorage _logStorage;

    public AzureTableLogStorageTests()
    {
        _mockTableClient = new Mock<TableClient>();
        var mockServiceClient = new Mock<TableServiceClient>();
        mockServiceClient
            .Setup(s => s.GetTableClient("WeatherLogs"))
            .Returns(_mockTableClient.Object);

        _logStorage = new AzureTableLogStorage(mockServiceClient.Object);
    }

    [Fact]
    public async Task SaveLogAsync_ShouldCallAddEntityAsync_WithCorrectEntity()
    {
        // Arrange
        var log = new WeatherLog
        {
            Id = "log1",
            Timestamp = DateTime.UtcNow,
            Status = "Success",
            ErrorMessage = null,
            BlobName = "blob-name"
        };

        TableEntity? capturedEntity = null;
        _mockTableClient
            .Setup(t => t.AddEntityAsync(It.IsAny<TableEntity>(), It.IsAny<CancellationToken>()))
            .Callback<TableEntity, CancellationToken>((entity, _) => capturedEntity = entity)
            .ReturnsAsync(Mock.Of<Response>());


        // Act
        await _logStorage.SaveLogAsync(log);

        // Assert
        Assert.NotNull(capturedEntity);
        Assert.Equal(log.Id, capturedEntity!.RowKey);
        Assert.Equal(log.Timestamp.ToString("yyyy-MM-dd"), capturedEntity.PartitionKey);
        Assert.Equal(log.Timestamp, capturedEntity["Timestamp"]);
        Assert.Equal("Success", capturedEntity["Status"]);
        Assert.Equal(string.Empty, capturedEntity["ErrorMessage"]);
        Assert.Equal("blob-name", capturedEntity["BlobName"]);
    }

    


}

using Azure.Storage.Blobs;
using Moq;
using Azure.Storage.Blobs.Models;
using TaskFirstRemote.Infrastructure.Services;
using Azure;

namespace TaskFirstRemoteTest
{
    public class AzureBlobPayloadStorageTests
    {
        private readonly Mock<BlobContainerClient> _mockContainer;
        private readonly Mock<BlobClient> _mockBlob;
        private readonly AzureBlobPayloadStorage _storage;

        public AzureBlobPayloadStorageTests()
        {
            _mockContainer = new Mock<BlobContainerClient>();

            _mockBlob = new Mock<BlobClient>();
            _mockContainer.Setup(c => c.GetBlobClient(It.IsAny<string>()))
                .Returns(_mockBlob.Object);

            _storage = new AzureBlobPayloadStorage(_mockContainer.Object);
        }


        [Fact]
        public async Task SavePayloadAsync_UploadsBlob()
        {
            // Arrange
            string id = "test-id";
            string content = "{\"key\": \"value\"}";

            _mockBlob.Setup(b => b.UploadAsync(
                It.IsAny<BinaryData>(),
                true,
                default)).ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());

            // Act
            await _storage.SavePayloadAsync(id, content);

            // Assert
            _mockContainer.Verify(c => c.GetBlobClient($"{id}.json"), Times.Once);
            _mockBlob.Verify(b => b.UploadAsync(
                It.Is<BinaryData>(d => d.ToString() == content),
                true,
                default), Times.Once);
        }

        [Fact]
        public async Task GetPayloadAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            string id = "not-found";

            _mockBlob.Setup(b => b.ExistsAsync(default))
                     .ReturnsAsync(Response.FromValue(false, null!));

            // Act
            var result = await _storage.GetPayloadAsync(id);

            // Assert
            Assert.Null(result);
        }
    }
}

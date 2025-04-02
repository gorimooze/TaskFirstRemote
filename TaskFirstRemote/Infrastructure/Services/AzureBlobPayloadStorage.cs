using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using TaskFirstRemote.Core.Interfaces;

namespace TaskFirstRemote.Infrastructure.Services
{
    public class AzureBlobPayloadStorage : IPayloadStorage
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobPayloadStorage(BlobContainerClient containerClient)
        {
            _containerClient = containerClient;
            _containerClient.CreateIfNotExists();
        }

        public async Task SavePayloadAsync(string id, string content)
        {
            var blob = _containerClient.GetBlobClient($"{id}.json");
            await blob.UploadAsync(BinaryData.FromString(content), overwrite: true);
        }

        public async Task<string?> GetPayloadAsync(string id)
        {
            var blob = _containerClient.GetBlobClient($"{id}.json");

            if (!await blob.ExistsAsync())
                return null;

            var result = await blob.DownloadContentAsync();
            return result.Value.Content.ToString();
        }
    }
}

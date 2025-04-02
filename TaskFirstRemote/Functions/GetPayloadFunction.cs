using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TaskFirstRemote.Core.Interfaces;

namespace TaskFirstRemote.Functions
{
    public class GetPayloadFunction
    {
        private readonly IPayloadStorage _payloadStorage;

        public GetPayloadFunction(IPayloadStorage payloadStorage)
        {
            _payloadStorage = payloadStorage;
        }

        [Function("GetPayload")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payload/{id}")] HttpRequestData req,
            string id)
        {
            var payload = await _payloadStorage.GetPayloadAsync(id);
            if (payload == null)
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteStringAsync("Payload not found.");
                return notFound;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(payload);
            return response;
        }
    }
}

using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System.Net;
using TaskFirstRemote.Core.Interfaces;

namespace TaskFirstRemote.Functions
{
    public class GetLogsFunction
    {
        private readonly ILogStorage _logStorage;

        public GetLogsFunction(ILogStorage logStorage)
        {
            _logStorage = logStorage;
        }

        [Function("GetLogs")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "logs")] HttpRequestData req)
        {
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            if (!DateTime.TryParse(query["from"], out var from) ||
                !DateTime.TryParse(query["to"], out var to))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid or missing 'from' or 'to' query parameters");
                return badResponse;
            }

            var logs = await _logStorage.GetLogsAsync(from, to);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(logs);
            return response;
        }
    }
}

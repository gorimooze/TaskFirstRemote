using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFirstRemote.Core.Interfaces;
using TaskFirstRemote.Core.Models;
using Microsoft.Azure.Functions.Worker;

namespace TaskFirstRemote.Functions
{
    public class WeatherTimerFunction
    {
        private readonly IWeatherService _weatherService;
        private readonly ILogStorage _logStorage;
        private readonly IPayloadStorage _payloadStorage;
        private readonly ILogger<WeatherTimerFunction> _logger;

        public WeatherTimerFunction(
            IWeatherService weatherService,
            ILogStorage logStorage,
            IPayloadStorage payloadStorage,
            ILogger<WeatherTimerFunction> logger)
        {
            _weatherService = weatherService;
            _logStorage = logStorage;
            _payloadStorage = payloadStorage;
            _logger = logger;
        }

        [Function("WeatherTimer")]
        public async Task Run([TimerTrigger("0 * * * * *")] TimerInfo timer)
        {
            var log = new WeatherLog
            {
                Timestamp = DateTime.UtcNow
            };

            try
            {
                var payload = await _weatherService.FetchWeatherDataAsync();

                await _payloadStorage.SavePayloadAsync(log.Id, payload);
                log.Status = "Success";
                log.BlobName = $"{log.Id}.json";
            }
            catch (Exception ex)
            {
                log.Status = "Failure";
                log.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error when receiving weather");
            }

            await _logStorage.SaveLogAsync(log);
        }
    }
}

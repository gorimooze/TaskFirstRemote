using TaskFirstRemote.Core.Interfaces;

namespace TaskFirstRemote.Infrastructure.Services
{
    public class OpenWeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenWeatherService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = Environment.GetEnvironmentVariable("WeatherApiKey")
                      ?? throw new InvalidOperationException("WeatherApiKey not found.");
        }

        public async Task<string> FetchWeatherDataAsync()
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q=London,uk&APPID={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}

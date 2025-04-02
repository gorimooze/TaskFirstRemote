using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using TaskFirstRemote.Infrastructure.Services;
using Xunit;

public class OpenWeatherServiceTests
{
    [Fact]
    public async Task FetchWeatherDataAsync_ReturnsWeatherData()
    {
        // Arrange
        var expectedContent = "{\"weather\":[{\"description\":\"clear sky\"}]}";

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
           .Protected()
           .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent(expectedContent),
           });

        var httpClient = new HttpClient(handlerMock.Object);

        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        Environment.SetEnvironmentVariable("WeatherApiKey", "dummy-api-key");

        var weatherService = new OpenWeatherService(httpClientFactoryMock.Object);

        // Act
        var result = await weatherService.FetchWeatherDataAsync();

        // Assert
        Assert.Equal(expectedContent, result);
    }

    [Fact]
    public void Constructor_Throws_WhenApiKeyNotFound()
    {
        // Arrange
        Environment.SetEnvironmentVariable("WeatherApiKey", null);
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new OpenWeatherService(httpClientFactoryMock.Object));

        Assert.Equal("WeatherApiKey not found.", exception.Message);
    }
}

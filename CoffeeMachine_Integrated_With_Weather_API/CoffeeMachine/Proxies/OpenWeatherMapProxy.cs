using CoffeeMachine.Infrastructure;
using CoffeeMachine.Models;

namespace CoffeeMachine.Proxies
{
    public interface IOpenWeatherMapProxy
    {
        Task<GeoCoordinates> GetGeoCoordinatesByZipAndContryCode(int ZipCode, string countryCode);
        Task<Weather> GetWeather(string lat, string lon);
    }

    public class OpenWeatherMapProxy : IOpenWeatherMapProxy
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;
        private readonly string _appId;

        public OpenWeatherMapProxy(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _baseUrl = configuration["api.openweathermap:baseurl"];
            _appId = configuration["api.openweathermap:appid"];
        }
        public async Task<GeoCoordinates> GetGeoCoordinatesByZipAndContryCode(int zipCode, string countryCode)
        {
            var url = $"{_baseUrl}/geo/1.0/zip?zip={zipCode},{countryCode}&appid={_appId}";

            var httpRequestMessage = HttpRequestMessageFactory.Create(HttpMethod.Get, url);

            var response = await _client.SendAsync(httpRequestMessage);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<GeoCoordinates>();
            }

            throw new InvalidOperationException($"Get Geo Coordinates By Zip Code faild with a status code of {response.StatusCode}");
        }

        public async Task<Weather> GetWeather(string lat, string lon)
        {
            var url = $"{_baseUrl}/data/3.0/onecall?lat={lat}&lon={lon}&units=metric&exclude=minutely,hourly,daily,alerts&appid={_appId}";

            var httpRequestMessage = HttpRequestMessageFactory.Create(HttpMethod.Get, url);

            var response = await _client.SendAsync(httpRequestMessage);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<Weather>();
            }

            throw new InvalidOperationException($"Get Weather faild with a status code of {response.StatusCode}");
        }
    }
}

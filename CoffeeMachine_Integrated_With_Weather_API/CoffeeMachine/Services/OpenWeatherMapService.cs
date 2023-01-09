using CoffeeMachine.Models;
using CoffeeMachine.Proxies;

namespace CoffeeMachine.Services
{
    public interface IOpenWeatherMapService
    {
        Task<Weather?> GetCurrentWeather(int zipCode, string countryCode);
    }

    public class OpenWeatherMapService : IOpenWeatherMapService
    {
        private readonly ILogger<IOpenWeatherMapService> _logger;
        private readonly IOpenWeatherMapProxy _openWeatherMapProxy;
        
        public OpenWeatherMapService(ILogger<IOpenWeatherMapService> logger, IOpenWeatherMapProxy openWeatherMapProxy)
        {
            _logger = logger;           
            _openWeatherMapProxy = openWeatherMapProxy;
        }

        public async Task<Weather?> GetCurrentWeather(int zipCode, string countryCode)
        {
            //request field validation
            if (zipCode <= 0 || string.IsNullOrWhiteSpace(countryCode))
            {
                _logger.LogError("Invalid ZipCode Or CountryCode");
                return null;
            }

            var coordinates = await _openWeatherMapProxy.GetGeoCoordinatesByZipAndContryCode(zipCode, countryCode);
            if(coordinates == null)
            {
                _logger.LogError($"No record found while retrieving GeoCoordinates for Zip code:{zipCode} and Country code:{countryCode}");
                return null;
            }

            return await _openWeatherMapProxy.GetWeather(coordinates.Lat, coordinates.Lon);
        }
    }
}

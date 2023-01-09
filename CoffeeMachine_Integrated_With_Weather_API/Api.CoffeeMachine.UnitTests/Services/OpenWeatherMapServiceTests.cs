using CoffeeMachine.Proxies;
using CoffeeMachine.Services;
using CoffeeMachine.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace Api.CoffeeMachine.UnitTests.Services
{
    public class OpenWeatherMapServiceTests
    {
        private readonly ILogger<IOpenWeatherMapService> _logger;
        private readonly IOpenWeatherMapProxy _openWeatherMapProxy;
        private readonly IOpenWeatherMapService _openWeatherMapService;

        public OpenWeatherMapServiceTests()
        {
            _logger = Substitute.For<ILogger<IOpenWeatherMapService>>();
            _openWeatherMapProxy = Substitute.For<IOpenWeatherMapProxy>();
            _openWeatherMapService = new OpenWeatherMapService(_logger, _openWeatherMapProxy);
        }

        [InlineData(-1, "")]
        [InlineData(0, " ")]
        [InlineData(1, null)]
        [Theory]
        public async void Should_Return_Null_For_Invalid_Request(int zipCode, string countryCode)
        {
            //Act
            var response = await _openWeatherMapService.GetCurrentWeather(zipCode, countryCode);

            //Assert
            response.Should().BeNull();
            _logger.ReceivedCalls()
                .Select(call => call.GetArguments())
                .Count(arguments => !((LogLevel)arguments[0]).Equals(LogLevel.Error)
                || !((IReadOnlyList<KeyValuePair<string, object>>)arguments[2]).Last().Value.ToString()
                .Contains("Invalid ZipCode Or CountryCode")).Equals(1);
        }

        [Fact]
        public async void Should_Return_Null_When_GetGeoCoordinatesByZipAndContryCode_Returns_Null()
        {
            //Arrange
            var zipCode = 1;
            var countryCode = "AU";
            _openWeatherMapProxy.GetGeoCoordinatesByZipAndContryCode(zipCode, countryCode).ReturnsNull();

            //Act
            var response = await _openWeatherMapService.GetCurrentWeather(zipCode, countryCode);

            //Assert
            response.Should().BeNull();
            _logger.ReceivedCalls()
                .Select(call => call.GetArguments())
                .Count(arguments => !((LogLevel)arguments[0]).Equals(LogLevel.Error)
                || !((IReadOnlyList<KeyValuePair<string, object>>)arguments[2]).Last().Value.ToString()
                .Contains($"No record found while retrieving GeoCoordinates for Zip code:{zipCode} and Country code:{countryCode}")).Equals(1);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_GetGeoCoordinatesByZipAndContryCode_Throws_Exception()
        {
            //Arrange
            var zipCode = 1;
            var countryCode = "AU";
            _openWeatherMapProxy.GetGeoCoordinatesByZipAndContryCode(zipCode, countryCode).Throws(new Exception());

            //Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _openWeatherMapService.GetCurrentWeather(zipCode, countryCode));
        }

        [Fact]
        public async Task Should_Throw_Exception_When_GetWeather_Throws_Exception()
        {
            //Arrange
            var zipCode = 1;
            var countryCode = "AU";
            _openWeatherMapProxy.GetGeoCoordinatesByZipAndContryCode(zipCode, countryCode).Returns(new GeoCoordinates
            {
                Lat = "123",
                Lon= "123"
            });
            _openWeatherMapProxy.GetWeather("123", "123").Throws(new Exception());

            //Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _openWeatherMapService.GetCurrentWeather(zipCode, countryCode));
        }

        [Fact]
        public async Task Should_Return_Success_Response_While_Calling_GetCurrentWeather()
        {
            //Arrange
            var zipCode = 1;
            var countryCode = "AU";
            var temperature = 21;
           _openWeatherMapProxy.GetGeoCoordinatesByZipAndContryCode(zipCode, countryCode).Returns(new GeoCoordinates
            {
                Lat = "123",
                Lon = "123"
            });
            _openWeatherMapProxy.GetWeather("123", "123").Returns(new Weather { Current = new Current { Temp = temperature.ToString() } });

            //Act
            var response = await _openWeatherMapService.GetCurrentWeather(zipCode, countryCode);

            //Assert
            response.Should().NotBeNull();
            response?.Current.Temp.Should().Be(temperature.ToString());
        }
    }
}

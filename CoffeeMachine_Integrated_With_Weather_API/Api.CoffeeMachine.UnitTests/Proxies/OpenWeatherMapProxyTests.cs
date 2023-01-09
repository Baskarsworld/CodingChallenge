using CoffeeMachine.Proxies;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Reflection.Emit;
using Xunit;

namespace Api.CoffeeMachine.UnitTests.Proxies
{
    public class OpenWeatherMapProxyTests
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private OpenWeatherMapProxy _openWeatherMapProxy;
        

        public OpenWeatherMapProxyTests()
        {
            _client = new HttpClient();
            _configuration = Substitute.For<IConfiguration>();
            _configuration["api.openweathermap:baseurl"].Returns("http://api.openweathermap.org");
            _openWeatherMapProxy = new OpenWeatherMapProxy(_client, _configuration);
        }

        [Fact]
        public async void Should_Return_Success_Response_While_Calling_GetWeather()
        {
            //Arrange
            var lat = "-33.8229";
            var lon = "151.0108";
            _configuration["api.openweathermap:appid"].Returns("a3f0b4a398d9427cf6a3b709cb308d4d");
            _openWeatherMapProxy = new OpenWeatherMapProxy(_client, _configuration);

            //Act
            var response = await _openWeatherMapProxy.GetWeather(lat, lon);

            //Assert
            response.Should().NotBeNull();
            response.TimeZone.Should().Be("Australia/Sydney");
            response.Lat.Should().Be(lat);
            response.Lon.Should().Be(lon);
            response.Current.Temp.Should().NotBeNullOrEmpty();
        }

        [InlineData("0", "151.0108", "a3f0b4a398d9427cf6a3b709cb308d4d")]
        [InlineData("-33.8229", "0", "a3f0b4a398d9427cf6a3b709cb308d4d") ]
        [InlineData("-33.8229", "151.0108", "0")]
        [Theory]
        public async void Should_Throw_Exception_For_Invalid_Requests_While_Calling_GetWeather(string lat, string lon, string appId)
        {
            //Arrange
            _configuration["api.openweathermap:appid"].Returns(appId);

            //Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _openWeatherMapProxy.GetWeather(lat, lon));            
        }

        [Fact]
        public async void Should_Return_Success_Response_While_Calling_GetGeoCoordinatesByZipAndContryCode()
        {
            //Arrange
            var zipCode = 2150;
            var countryCode = "AU";
            _configuration["api.openweathermap:appid"].Returns("a3f0b4a398d9427cf6a3b709cb308d4d");
            _openWeatherMapProxy = new OpenWeatherMapProxy(_client, _configuration);

            //Act
            var response = await _openWeatherMapProxy.GetGeoCoordinatesByZipAndContryCode(zipCode, countryCode);

            //Assert
            response.Should().NotBeNull();
            response.Lat.Should().NotBeNull();
            response.Lon.Should().NotBeNull();
            response.Zip.Should().Be(zipCode.ToString());
        }

        [InlineData(0, "AU", "a3f0b4a398d9427cf6a3b709cb308d4d")]
        [InlineData(2150, "ABC", "a3f0b4a398d9427cf6a3b709cb308d4d")]
        [InlineData(2150, "AU", "0")]
        [Theory]
        public async void Should_Throw_Exception_For_Invalid_Requests_While_Calling_GetGeoCoordinatesByZipAndContryCode(int zipCode, string countryCode, string appId)
        {
            //Arrange
            _configuration["api.openweathermap:appid"].Returns(appId);
            _openWeatherMapProxy = new OpenWeatherMapProxy(_client, _configuration);

            //Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _openWeatherMapProxy.GetGeoCoordinatesByZipAndContryCode(zipCode, countryCode));
        }
    }
}

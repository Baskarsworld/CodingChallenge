using Api.CoffeeMachine.Common;
using CoffeeMachine.Common;
using CoffeeMachine.EFCore;
using CoffeeMachine.Models;
using CoffeeMachine.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace Api.CoffeeMachine.UnitTests.Services
{
    public class CoffeeMachineServiceTests
    {
        private readonly ILogger<ICoffeeMachineService> _logger;
        private readonly ICoffeeMachineDetailRepository _coffeeMachineDetailsRepository;
        private readonly ICoffeeMachineUsedCountRepository _coffeeMachineUsedCountRepository;
        private readonly ICoffeeMachineService _coffeeMachineService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CoffeeMachineServiceTests()
        {
            _logger = Substitute.For<ILogger<ICoffeeMachineService>>();
            _coffeeMachineDetailsRepository = Substitute.For<ICoffeeMachineDetailRepository>();
            _coffeeMachineUsedCountRepository = Substitute.For<ICoffeeMachineUsedCountRepository>();
            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
            _coffeeMachineService = new CoffeeMachineService(_logger, _coffeeMachineDetailsRepository,
                _coffeeMachineUsedCountRepository, _dateTimeProvider);
        }

        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [Theory]
        public void Should_Return_BadRequest_Error_For_Invalid_Request(string coffeeMachineId)
        {
            //Act
            var response = _coffeeMachineService.GetBrewCoffee(coffeeMachineId);
            var result = response as ContentResult;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<ContentResult>(response);
            Assert.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
            _logger.ReceivedCalls()
                .Select(call => call.GetArguments())
                .Count(arguments => !((LogLevel)arguments[0]).Equals(LogLevel.Error)
                || !((IReadOnlyList<KeyValuePair<string, object>>)arguments[2]).Last().Value.ToString()
                .Contains("Invalid CoffeeMachineId")).Equals(1);
        }

        [Fact]
        public void Should_Return_NotFound_Error_When_RetrieveCoffeeMachineById_Return_Null_Response()
        {
            //Arrange
            var coffeeMachineId = "1";
            _coffeeMachineDetailsRepository.RetrieveCoffeeMachineById(coffeeMachineId).ReturnsNull();

            //Act
            var response = _coffeeMachineService.GetBrewCoffee(coffeeMachineId);
            var result = response as ContentResult;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<ContentResult>(response);
            Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
            _logger.ReceivedCalls()
               .Select(call => call.GetArguments())
               .Count(arguments => !((LogLevel)arguments[0]).Equals(LogLevel.Error)
               || !((IReadOnlyList<KeyValuePair<string, object>>)arguments?[2]).Last().Value.ToString()
               .Contains("Record Not Found for CoffeeMachineId : " + coffeeMachineId)).Equals(1);
        }

        [Fact]
        public void Should_Return_InternalServer_Error_When_RetrieveCoffeeMachineById_Throws_Exception()
        {
            //Arrange
            var coffeeMachineId = "1";
            _coffeeMachineDetailsRepository.RetrieveCoffeeMachineById(coffeeMachineId).Throws(new Exception());

            //Act
            var response = _coffeeMachineService.GetBrewCoffee(coffeeMachineId);
            var result = response as ContentResult;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<ContentResult>(response);
            Assert.Equal(StatusCodes.Status500InternalServerError, result?.StatusCode);
            _logger.ReceivedCalls()
               .Select(call => call.GetArguments())
               .Count(arguments => !((LogLevel)arguments[0]).Equals(LogLevel.Error)
               || !((IReadOnlyList<KeyValuePair<string, object>>)arguments?[2]).Last().Value.ToString()
               .Contains("GetBrewCoffee: Exception occurred for the CoffeeMachineId: " + coffeeMachineId)).Equals(1);
        }

        [Fact]
        public void Should_Return_IAmTeaPot_Error_For_April_First()
        {
            //Arrange
            var coffeeMachineId = "1";
            _coffeeMachineDetailsRepository.RetrieveCoffeeMachineById(coffeeMachineId).Returns(new CoffeeMachineDetail
            {
                Id = 1
            });
            _dateTimeProvider.GetCurrentTime().Returns(new DateTime(2023, 4, 1));

            //Act
            var response = _coffeeMachineService.GetBrewCoffee(coffeeMachineId);
            var result = response as ContentResult;
           
            //Assert
            Assert.NotNull(response);
            Assert.IsType<ContentResult>(response);
            Assert.Equal(StatusCodes.Status418ImATeapot, result?.StatusCode);
            _logger.ReceivedCalls()
               .Select(call => call.GetArguments())
               .Count(arguments => !((LogLevel)arguments[0]).Equals(LogLevel.Error)
               || !((IReadOnlyList<KeyValuePair<string, object>>)arguments?[2]).Last().Value.ToString()
               .Contains("The date is April 1st for CoffeeMachineId:" + coffeeMachineId)).Equals(1);
        }

        [Fact]
        public void Should_Return_ServiceUnavailable_Error_When_GetBrewCoffee_Endpoint_Called_At_5th_Time()
        {
            //Arrange
            var coffeeMachineId = "1";
            _coffeeMachineDetailsRepository.RetrieveCoffeeMachineById(coffeeMachineId)
                .Returns(new CoffeeMachineDetail
                {
                    Id = 1
                });
            _coffeeMachineUsedCountRepository.RetrieveCoffeeMachineUsedCountById(coffeeMachineId)
                .Returns(new CoffeeMachineUsedCount
                {
                    Id = 1,
                    UsedCount= 4
                });
            _dateTimeProvider.GetCurrentTime().Returns(new DateTime(2023, 3, 2));

            //Act
            var response = _coffeeMachineService.GetBrewCoffee(coffeeMachineId);
            var result = response as ContentResult;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<ContentResult>(response);
            Assert.Equal(StatusCodes.Status503ServiceUnavailable, result?.StatusCode);
            _logger.ReceivedCalls()
               .Select(call => call.GetArguments())
               .Count(arguments => !((LogLevel)arguments[0]).Equals(LogLevel.Error)
               || !((IReadOnlyList<KeyValuePair<string, object>>)arguments?[2]).Last().Value.ToString()
               .Contains("Coffee machine called for 5th time for the CoffeeMachineId : " + coffeeMachineId)).Equals(1);
        }       
      
        [Fact]
        public void Should_Return_Success_Response_For_Valid_Request_While_Calling_GetBrewCoffee()
        {
            //Arrange
            var coffeeMachineId = "1";
            _coffeeMachineDetailsRepository.RetrieveCoffeeMachineById(coffeeMachineId)
                .Returns(new CoffeeMachineDetail
                {
                    Id = 1,
                    Address = new Address
                    {
                        ZipCode = 2150,
                        CountryCode = "AU"
                    }
                });
            _coffeeMachineUsedCountRepository.RetrieveCoffeeMachineUsedCountById(coffeeMachineId)
                .Returns(new CoffeeMachineUsedCount
                {
                    Id = 1,
                    UsedCount = 1
                });
           
            _dateTimeProvider.GetCurrentTime().Returns(new DateTime(2023, 3, 2));

            var expectedResult =  new BrewCoffee
            {
                Message = Constants.HotCoffeeReadyMessage,
                Prepared = _dateTimeProvider.GetCurrentTime().ToString("o")
            };

            //Act
            var response = _coffeeMachineService.GetBrewCoffee(coffeeMachineId);
            var result = response as OkObjectResult;

            //Assert
            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response);
            Assert.IsType<BrewCoffee>(result?.Value);
            Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
            result?.Value.Should().BeEquivalentTo(expectedResult);
        }
    }
}

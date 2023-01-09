using CoffeeMachine.Common;
using CoffeeMachine.Controllers;
using CoffeeMachine.Models;
using CoffeeMachine.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using FluentAssertions;

namespace Api.CoffeeMachine.UnitTests.Controllers
{
    public class CoffeeMachineControllerTests
    {
        private readonly ICoffeeMachineService _coffeeMachineService;
        private readonly CoffeeMachineController _coffeeMachineController;

        public CoffeeMachineControllerTests()
        {
            _coffeeMachineService = Substitute.For<ICoffeeMachineService>();
            _coffeeMachineController = new CoffeeMachineController(_coffeeMachineService);
        }

        [Fact] 
        public void Should_Get_BrewCoffee()
        {
            //Arrange
            var coffeeMachineId = "1";
            var expectedResult = new BrewCoffee
            {
                Message =Constants.HotCoffeeReadyMessage,
                Prepared =DateTime.Now.ToString("o")
            };
            _coffeeMachineService.GetBrewCoffee(Arg.Any<string>())
                .Returns(new OkObjectResult(expectedResult));

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

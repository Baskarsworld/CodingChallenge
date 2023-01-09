using CoffeeMachine.EFCore;
using FluentAssertions;
using Xunit;

namespace Api.CoffeeMachine.UnitTests.EFCore
{
    public class CoffeeMachineDetailRepositoryTests
    {
        public static readonly ICoffeeMachineDetailRepository _coffeeMachineDetailRepository;

        static CoffeeMachineDetailRepositoryTests()
        {
            if (_coffeeMachineDetailRepository == null)
            {
                _coffeeMachineDetailRepository = new CoffeeMachineDetailRepository();
            }
        }

        [Fact]
        public void Should_Return_All_CoffeeMachine_Details_While_Calling_GetCoffeeMachines()
        {
            //Act
            var response = _coffeeMachineDetailRepository.GetCoffeeMachines();

            //Assert
            response.Should().NotBeNull();
            response.Count.Should().Be(2);
            response[0].Id.Should().Be(1);
            response[1].Id.Should().Be(2);
        }

        [Fact]
        public void Should_Return_Null_Response_For_Invalid_MachineId_While_Calling_RetrieveCoffeeMachineById()
        {
            //Act
            var response = _coffeeMachineDetailRepository.RetrieveCoffeeMachineById("0");

            //Assert
            response.Should().BeNull();
          
        }

        [Fact]
        public void Should_Proper_Response_For_Valid_MachineId_While_Calling_RetrieveCoffeeMachineById()
        {
            //Act
            var response = _coffeeMachineDetailRepository.RetrieveCoffeeMachineById("1");

            //Assert
            response.Should().NotBeNull();           
            response?.Id.Should().Be(1);
            response?.Address.AddressId.Should().Be(1);
        }
    }
}

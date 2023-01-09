using CoffeeMachine.EFCore;
using FluentAssertions;
using Xunit;

namespace Api.CoffeeMachine.UnitTests.EFCore
{
    public class CoffeeMachineUsedCountRepositoryTests
    {
        public readonly ICoffeeMachineUsedCountRepository _coffeeMachineUsedCountRepository;

        public CoffeeMachineUsedCountRepositoryTests()
        {
            _coffeeMachineUsedCountRepository = new CoffeeMachineUsedCountRepository();
        }

        [Fact]
        public void Should_Return_Null_When_No_Record_Available_While_Calling_RetrieveCoffeeMachineUsedCountById()
        {
            //Act
            var response = _coffeeMachineUsedCountRepository.RetrieveCoffeeMachineUsedCountById("1");

            //Assert
            response.Should().BeNull();
        }

        [Fact]
        public void Should_Insert_Into_DB_While_Calling_AddOrModifyCoffeeMachineUsedCount_For_First_Time()
        {
            //Arrange
            var coffeeMachineId = 1;

            //Act
            _coffeeMachineUsedCountRepository.AddOrModifyCoffeeMachineUsedCount(coffeeMachineId.ToString());
            var response = _coffeeMachineUsedCountRepository.RetrieveCoffeeMachineUsedCountById(coffeeMachineId.ToString());

            //Assert
            response.Should().NotBeNull();
            response?.CoffeeMachineId.Should().Be(coffeeMachineId);
            response?.UsedCount.Should().Be(1);
        }

        [Fact]
        public void Should_Update_Existing_Record_While_Calling_AddOrModifyCoffeeMachineUsedCount_For_Second_Time()
        {
            //Arrange
            var coffeeMachineId = 2;

            //Act
            _coffeeMachineUsedCountRepository.AddOrModifyCoffeeMachineUsedCount(coffeeMachineId.ToString()); //1st time
            _coffeeMachineUsedCountRepository.AddOrModifyCoffeeMachineUsedCount(coffeeMachineId.ToString()); //2nd time
            var response = _coffeeMachineUsedCountRepository.RetrieveCoffeeMachineUsedCountById(coffeeMachineId.ToString());

            //Assert
            response.Should().NotBeNull();
            response?.CoffeeMachineId.Should().Be(coffeeMachineId);
            response?.UsedCount.Should().Be(2);
        }

    }
}

using Api.CoffeeMachine.Common;
using CoffeeMachine.EFCore;
using CoffeeMachine.Models;
using Microsoft.AspNetCore.Mvc;
using static CoffeeMachine.Common.Constants;

namespace CoffeeMachine.Services
{
    public interface ICoffeeMachineService
    {
        IActionResult GetBrewCoffee(string coffeeMachineId);
    }

    public class CoffeeMachineService : ICoffeeMachineService
    {
        private readonly ILogger<ICoffeeMachineService> _logger;
        private readonly ICoffeeMachineDetailRepository _coffeeMachineDetailsRepository;
        private readonly ICoffeeMachineUsedCountRepository _coffeeMachineUsedCountRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CoffeeMachineService(ILogger<ICoffeeMachineService> logger,
                                    ICoffeeMachineDetailRepository coffeeMachineDetailsRepository,
                                    ICoffeeMachineUsedCountRepository coffeeMachineUsedCountRepository,
                                    IDateTimeProvider dateTimeProvider)
        {
            _logger = logger;
            _coffeeMachineDetailsRepository = coffeeMachineDetailsRepository;
            _coffeeMachineUsedCountRepository = coffeeMachineUsedCountRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public IActionResult GetBrewCoffee(string coffeeMachineId)
        {
            try
            {
                //request field validation
                if (string.IsNullOrWhiteSpace(coffeeMachineId))
                {
                    _logger.LogError("Invalid CoffeeMachineId");

                    return new ContentResult
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                //retreive coffee machine detail
                var coffeeMachine = _coffeeMachineDetailsRepository.RetrieveCoffeeMachineById(coffeeMachineId);
                if (coffeeMachine == null)
                {
                    _logger.LogError("Record Not Found for CoffeeMachineId : " + coffeeMachineId);

                    return new ContentResult
                    {
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                //if the date is April 1st, then return 418 I’m a teapot 
                if (_dateTimeProvider.GetCurrentTime().Month == (int)Months.April && _dateTimeProvider.GetCurrentTime().Day == 1)
                {
                    _logger.LogInformation("The date is April 1st for CoffeeMachineId:" + coffeeMachineId);

                    return new ContentResult
                    {
                        StatusCode = StatusCodes.Status418ImATeapot
                    };
                }

                // check the 5th api call for the given machine id
                var usedCount = RetrieveCoffeeMachineUsedCount(coffeeMachineId);
                if (usedCount + 1 > 4)
                {
                    _logger.LogError("Coffee machine called for 5th time for the CoffeeMachineId : " + coffeeMachineId);

                    return new ContentResult
                    {
                        StatusCode = StatusCodes.Status503ServiceUnavailable
                    };
                }
                else
                {
                    _coffeeMachineUsedCountRepository.AddOrModifyCoffeeMachineUsedCount(coffeeMachineId);
                }
              
                return new OkObjectResult(new BrewCoffee
                {
                    Message = HotCoffeeReadyMessage,
                    Prepared = _dateTimeProvider.GetCurrentTime().ToString("o") //ISO-8601 formate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("GetBrewCoffee: Exception occurred for the CoffeeMachineId: " + coffeeMachineId, ex);
                return new ContentResult
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        private int RetrieveCoffeeMachineUsedCount(string coffeeMachineId)
        {
            var result = _coffeeMachineUsedCountRepository.RetrieveCoffeeMachineUsedCountById(coffeeMachineId);
            return result?.UsedCount ?? 0;
        }
    }
}

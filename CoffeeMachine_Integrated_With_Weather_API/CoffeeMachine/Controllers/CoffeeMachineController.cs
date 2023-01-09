using CoffeeMachine.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeMachine.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoffeeMachineController : ControllerBase
    {
        private readonly ICoffeeMachineService _coffeeMachineService;

        public CoffeeMachineController(ICoffeeMachineService coffeeMachineService)
        {
            _coffeeMachineService = coffeeMachineService;
        }

        /// <summary>
        /// Get Brew Coffee from inter connected coffee machines.
        /// This endpoint can serve/control more than 1 coffee machines at a time, 
        /// it will help when Coffeeshop grows with many coffee machines in future.
        /// </summary>
        /// <param name="coffeeMachineId">Unique id configured for each coffee machine. Sample values are 1,2</param>
        /// <returns>Status of coffee prepration</returns>
        [HttpGet("brew-coffee")]
        public async Task<IActionResult> Get(string coffeeMachineId = "1") //the first coffeeMachineId is set as default value for testing purpose. It will be removed in Prod environment.
        {
           return await _coffeeMachineService.GetBrewCoffee(coffeeMachineId);
        }
    }
}
using CoffeeMachine.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CoffeeMachine.EFCore
{
    public interface ICoffeeMachineUsedCountRepository
    {
        public CoffeeMachineUsedCount? RetrieveCoffeeMachineUsedCountById(string coffeeMachineId);
        public void AddOrModifyCoffeeMachineUsedCount(string coffeeMachineId);
    }

    public class CoffeeMachineUsedCountRepository : ICoffeeMachineUsedCountRepository
    {
        public CoffeeMachineUsedCountRepository()
        {          
        }

        public CoffeeMachineUsedCount? RetrieveCoffeeMachineUsedCountById(string coffeeMachineId)
        {
            int.TryParse(coffeeMachineId, out int machineId);

            using (var context = new ApiContext())
            {
                //Note: if needed, here we can have a logic to reset the used count based on a time interval(ex: after 4 hours, 24 hours and etc)

                return context?.CoffeeMachineUsedCount?.FirstOrDefault(item => item.Id == machineId);
            }
        }

        public void AddOrModifyCoffeeMachineUsedCount(string coffeeMachineId)
        {
            int.TryParse(coffeeMachineId, out int machineId);

            using (var context = new ApiContext())
            {
                var result = context?.CoffeeMachineUsedCount?.FirstOrDefault(item => item.Id == machineId);
                if(result != null) // update
                {
                    result.UsedCount++;
                    result.LastModified = DateTime.Now;
                    context?.SaveChanges();
                } 
                else // add
                {
                    context?.Add(new CoffeeMachineUsedCount
                    {
                         CoffeeMachineId = machineId,
                         UsedCount = 1,
                         LastModified= DateTime.Now
                    });
                    context?.SaveChanges();
                }                
            }
        }
    }
}

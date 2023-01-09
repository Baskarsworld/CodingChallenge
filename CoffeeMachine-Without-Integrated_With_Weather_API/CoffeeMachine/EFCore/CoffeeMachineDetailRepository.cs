using CoffeeMachine.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace CoffeeMachine.EFCore
{
    public interface ICoffeeMachineDetailRepository
    {
        public List<CoffeeMachineDetail> GetCoffeeMachines();
        public CoffeeMachineDetail? RetrieveCoffeeMachineById(string coffeeMachineId);
       
    }

    public class CoffeeMachineDetailRepository : ICoffeeMachineDetailRepository
    {
        public CoffeeMachineDetailRepository()
        {
            using (var context = new ApiContext())
            {
                var coffeeMachines = new List<CoffeeMachineDetail>
                {
                    new CoffeeMachineDetail
                    {
                        Id= 1,
                        Model = "EN85WSOLO",
                        Name = "Nespresso",
                        Address = new Address
                        {
                            AddressId = 1,                            
                            Unit = "101",
                            Street = "18 Sorrell street",
                            Area = "Parramatta",
                            ZipCode = 2150,
                            City = "Sydney",
                            CountryCode = "AU",
                            CoffeeMachineId = 1
                        }
                    },
                    new CoffeeMachineDetail
                    { 
                        Id= 2,
                        Model = "EMM5400SS",
                        Name = "Sunbeam",
                        Address = new Address
                        {
                            AddressId = 2,
                            Unit = "501",
                            Street = "216 Harris street",
                            Area = "Pyrmont",
                            ZipCode = 2009,
                            City = "Sydney",
                            CountryCode = "AU",
                            CoffeeMachineId = 2
                        }
                    }
                };
                context.CoffeeMachines.AddRange(coffeeMachines);
                context.SaveChanges();
            }
        }

        public List<CoffeeMachineDetail> GetCoffeeMachines()
        {
            using (var context = new ApiContext())
            {
                return context.CoffeeMachines.Include(item => item.Address).ToList();
            }
        }

        public CoffeeMachineDetail? RetrieveCoffeeMachineById(string coffeeMachineId)
        {
            int.TryParse(coffeeMachineId, out int machineId);

            using (var context = new ApiContext())
            {
                return context?.CoffeeMachines?.Include(item => item.Address)?.FirstOrDefault(item => item.Id == machineId);
            }
        }
    }
}

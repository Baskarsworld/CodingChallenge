using Microsoft.EntityFrameworkCore;
using CoffeeMachine.Models;

namespace CoffeeMachine.EFCore
{
    public class ApiContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //InMemoryDatabase is used for development purpose, it will be replaced with actual SQL DB in Prod environment
            optionsBuilder.UseInMemoryDatabase(databaseName: "CoffeeMachineDb");
        }
        public DbSet<CoffeeMachineDetail> CoffeeMachines { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<CoffeeMachineUsedCount> CoffeeMachineUsedCount { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CoffeeMachineDetail>()
                .HasOne(a => a.Address)
                .WithOne(c => c.CoffeeMachineDetail)
                .HasForeignKey<Address>(a => a.CoffeeMachineId);        
        }
    }
}
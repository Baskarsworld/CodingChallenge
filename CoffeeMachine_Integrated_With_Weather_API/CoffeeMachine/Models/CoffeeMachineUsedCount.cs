namespace CoffeeMachine.Models
{
    public class CoffeeMachineUsedCount
    {
        public int Id { get; set; }
        public int CoffeeMachineId { get; set; }
        public int UsedCount { get; set; }
        public DateTime LastModified { get; set; }
    }
}

namespace CoffeeMachine.Models
{
    public class CoffeeMachineDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public int AddressId { get; set; }
        public string Unit { get; set; }
        public string Street { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public int ZipCode { get; set; }
        public int CoffeeMachineId { get; set; }
        public CoffeeMachineDetail CoffeeMachineDetail { get; set; }
    }
}

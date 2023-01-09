namespace CoffeeMachine.Models
{
    public class Weather
    {
       public string Lat { get; set; }
       public string Lon { get; set; }
       public string TimeZone { get; set; }
       public string TimezoneOffset { get; set; }
       public Current Current { get; set; }       

    }

    public class Current
    {
        public string Dt { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public string Temp { get; set; }
        public string Feels_like { get; set; }
        public string Pressure { get; set; }
        public string Humidity { get; set; }
        public string Dew_point { get; set; }
        public string Uvi { get; set; }
        public string Clouds { get; set; }
        public string Visibility { get; set; }
        public string Wind_speed { get; set; }
        public string Wind_deg { get; set; }
        public string Wind_gust { get; set; }
        public List<WeatherDetail> Weather { get; set; }        
    }

    public class WeatherDetail
    {
        public string Id { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }       
}

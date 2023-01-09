using CoffeeMachine.Proxies;

namespace CoffeeMachine.Infrastructure.Configuration
{
    public static class HttpClientConfiguration
    {
        public static IServiceCollection ConfigurationHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient<IOpenWeatherMapProxy, OpenWeatherMapProxy>();

            return services;
        }
    }
}

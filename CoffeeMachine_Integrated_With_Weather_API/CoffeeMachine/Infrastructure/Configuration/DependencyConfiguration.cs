using Api.CoffeeMachine.Common;
using CoffeeMachine.EFCore;
using CoffeeMachine.Services;

namespace CoffeeMachine.Infrastructure.Configuration
{
    public static class DependencyConfiguration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<ICoffeeMachineService, CoffeeMachineService>();
            services.AddSingleton<ICoffeeMachineDetailRepository, CoffeeMachineDetailRepository>();
            services.AddScoped<ICoffeeMachineUsedCountRepository, CoffeeMachineUsedCountRepository>();
            services.AddScoped<IOpenWeatherMapService, OpenWeatherMapService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();            
            return services;
        }
    }
}

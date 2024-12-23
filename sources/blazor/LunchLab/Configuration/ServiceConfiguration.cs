using Microsoft.Extensions.Options;
using PayloadClient.Configuration;
using PayloadClient.Interfaces;
using PayloadClient.Repositories;
using PayloadClient.Repositories.Collections;

namespace LunchLab.Configuration;

public static class ServiceConfiguration
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PayloadOptions>(configuration.GetSection("Payload"));
        
        // Configure named HttpClient
        services.AddHttpClient("PayloadClient", (serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<PayloadOptions>>().Value;
            if (string.IsNullOrEmpty(options.BaseUrl))
            {
                throw new InvalidOperationException("Payload BaseUrl is not configured");
            }
            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.ApiKey}");
        });

        // Register repositories
        services.AddTransient(typeof(IPayloadRepository<>), typeof(PayloadRepository<>));
        services.AddTransient(typeof(IPayloadGlobalRepository<>), typeof(PayloadGlobalRepository<>));
        services.AddTransient<IRestaurantRepository, RestaurantRepository>();
        services.AddTransient<IMenuItemRepository, MenuItemRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IPollsRepository, PollsRepository>();
    }
}
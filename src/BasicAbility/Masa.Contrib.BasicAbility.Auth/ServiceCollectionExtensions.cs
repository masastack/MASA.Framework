

namespace Masa.Contrib.BasicAbility.Auth;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthClient(this IServiceCollection services, string authServiceBaseAddress)
    {
        ArgumentNullException.ThrowIfNull(authServiceBaseAddress, nameof(authServiceBaseAddress));

        return services.AddAuthClient(callerOptions =>
        {
            callerOptions.UseHttpClient(builder =>
            {
                builder.Name = DEFAULT_CLIENT_NAME;
                builder.Configure = opt => opt.BaseAddress = new Uri(authServiceBaseAddress);
            });
        });
    }

    public static IServiceCollection AddAuthClient(this IServiceCollection services, Action<CallerOptions> callerOptions)
    {
        ArgumentNullException.ThrowIfNull(callerOptions, nameof(callerOptions));

        services.AddCaller(callerOptions);

        services.AddScoped<IAuthClient>(serviceProvider =>
        {
            var callProvider = serviceProvider.GetRequiredService<ICallerFactory>().CreateClient(DEFAULT_CLIENT_NAME);
            var authClient = new AuthClient(callProvider);
            return authClient;
        });

        return services;
    }
}


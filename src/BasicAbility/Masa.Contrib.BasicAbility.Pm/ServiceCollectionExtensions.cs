namespace Masa.Contrib.BasicAbility.Pm;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddPmClient(this IServiceCollection services, string pmServerBaseAddress)
    {
        if (string.IsNullOrWhiteSpace(pmServerBaseAddress))
        {
            throw new ArgumentNullException(nameof(pmServerBaseAddress));
        }

        return services.AddPmClient(callerOptions =>
        {
            callerOptions.UseHttpClient(builder =>
            {
                builder.Name = DEFAULT_CLIENT_NAME;
                builder.Configure = opt => opt.BaseAddress = new Uri(pmServerBaseAddress);
            });
        });
    }

    public static IServiceCollection AddPmClient(this IServiceCollection services, Action<CallerOptions> callerOptions)
    {
        ArgumentNullException.ThrowIfNull(callerOptions, nameof(callerOptions));

        if (services.Any(service => service.ServiceType == typeof(IPmClient)))
            return services;

        services.AddCaller(callerOptions.Invoke);

        services.AddSingleton<IPmClient>(serviceProvider =>
        {
            var callProvider = serviceProvider.GetRequiredService<ICallerFactory>().CreateClient(DEFAULT_CLIENT_NAME);
            var pmCaching = new PmClient(callProvider);
            return pmCaching;
        });

        return services;
    }
}

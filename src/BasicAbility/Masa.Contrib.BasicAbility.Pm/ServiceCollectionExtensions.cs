namespace Masa.Contrib.BasicAbility.Pm;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPmClient(this IServiceCollection services, Action<CallerOptions> callerOptions)
    {
        if (services.Any(service => service.ServiceType.Equals(typeof(IPmClient))))
            return services;

        services.AddCaller(options =>
        {
            callerOptions.Invoke(options);
        });

        services.AddSingleton<IPmClient>(serviceProvider =>
        {
            var callProvider = serviceProvider.GetRequiredService<ICallerFactory>().CreateClient(DEFAULT_CLIENT_NAME);
            var pmCaching = new PmClient(callProvider);
            return pmCaching;
        });

        return services;
    }
}

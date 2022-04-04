
namespace Masa.Contrib.BasicAbility.Pm
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPmCaching(this IServiceCollection services, Action<CallerOptions> callerOptions)
        {
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
        }
    }
}

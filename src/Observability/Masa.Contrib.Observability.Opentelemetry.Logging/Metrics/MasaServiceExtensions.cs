namespace Microsoft.Extensions.DependencyInjection;

public static partial class MasaServiceExtensions
{
    public static IServiceCollection AddMasaMetrics(this IServiceCollection services, Action<MeterProviderBuilder> builderConfiger)
    {
        services.AddOpenTelemetryMetrics(builder =>
           {
               builder
               .AddRuntimeMetrics()
               .AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation();

               if (builderConfiger != null)
                   builderConfiger.Invoke(builder);
           }
        );

        return services;
    }
}

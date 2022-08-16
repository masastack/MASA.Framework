// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation;

public static class DispatcherOptionsExtensions
{
    /// <summary>
    /// It is not recommended to use directly here, please use UseIsolationUoW
    /// </summary>
    /// <param name="eventBusBuilder"></param>
    /// <param name="isolationBuilder"></param>
    /// <returns></returns>
    public static IEventBusBuilder UseIsolation(this IEventBusBuilder eventBusBuilder, Action<IsolationBuilder> isolationBuilder)
    {
        eventBusBuilder.Services.AddIsolation(isolationBuilder);
        return eventBusBuilder;
    }

    /// <summary>
    /// It is not recommended to use directly here, please use UseIsolationUoW
    /// </summary>
    /// <param name="options"></param>
    /// <param name="isolationBuilder"></param>
    /// <returns></returns>
    public static IDispatcherOptions UseIsolation(this IDispatcherOptions options, Action<IsolationBuilder> isolationBuilder)
    {
        options.Services.AddIsolation(isolationBuilder);
        return options;
    }

    private static void AddIsolation(this IServiceCollection services, Action<IsolationBuilder> isolationBuilder)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(isolationBuilder);

        if (services.Any(service => service.ImplementationType == typeof(IsolationProvider)))
            return;

        services.AddSingleton<IsolationProvider>();

        IsolationBuilder builder = new IsolationBuilder(services);
        isolationBuilder.Invoke(builder);

        if (services.Count(service =>
                service.ServiceType == typeof(ITenantContext) ||
                service.ServiceType == typeof(IEnvironmentContext)) < 1)
            throw new NotSupportedException("Tenant isolation and environment isolation use at least one");

        services.AddHttpContextAccessor();

        services
            .TryAddConfigure<IsolationDbConnectionOptions>()
            .AddTransient(typeof(IMiddleware<>), typeof(IsolationMiddleware<>))
            .TryAddSingleton<IDbConnectionStringProvider, IsolationDbContextProvider>();

        if (services.Any(service => service.ServiceType == typeof(IConnectionStringProvider)))
            services.Replace(new ServiceDescriptor(typeof(IConnectionStringProvider), typeof(DefaultDbIsolationConnectionStringProvider), ServiceLifetime.Scoped));
        else
            services.TryAddScoped<IConnectionStringProvider, DefaultDbIsolationConnectionStringProvider>();
    }

    private static IServiceCollection TryAddConfigure<TOptions>(
        this IServiceCollection services)
        where TOptions : class
    {
        services.AddOptions();
        var serviceProvider = services.BuildServiceProvider();
        IConfiguration? configuration = serviceProvider.GetService<IMasaConfiguration>()?.Local ??
            serviceProvider.GetService<IConfiguration>();

        if (configuration == null)
            return services;

        string name = Options.DefaultName;
        services.TryAddSingleton<IOptionsChangeTokenSource<TOptions>>(
            new ConfigurationChangeTokenSource<TOptions>(name, configuration));
        services.TryAddSingleton<IConfigureOptions<TOptions>>(new NamedConfigureFromConfigurationOptions<TOptions>(
            name,
            configuration, _ =>
            {
            }));
        return services;
    }

    private sealed class IsolationProvider
    {
    }
}

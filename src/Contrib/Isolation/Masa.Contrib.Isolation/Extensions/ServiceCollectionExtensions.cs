// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

internal static class ServiceCollectionExtensions
{
    public static void AddIsolation(this IServiceCollection services, Action<IsolationBuilder> isolationBuilder)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(isolationBuilder);

        if (services.Any(service => service.ImplementationType == typeof(IsolationProvider)))
            return;

        services.AddSingleton<IsolationProvider>();

        IsolationBuilder builder = new IsolationBuilder(services);
        isolationBuilder.Invoke(builder);

        if (!services.Any(service =>
                service.ServiceType == typeof(IMultiTenantContext) ||
                service.ServiceType == typeof(IMultiEnvironmentContext)))
            throw new NotSupportedException("Tenant isolation and environment isolation use at least one");

        services.AddHttpContextAccessor();

        services
            .TryAddConfigure<IsolationDbConnectionOptions>()
            .AddTransient(typeof(IMiddleware<>), typeof(IsolationMiddleware<>))
            .TryAddSingleton<IDbConnectionStringProvider, IsolationDbContextProvider>();

        if (services.Any(service => service.ServiceType == typeof(IConnectionStringProvider)))
            services.Replace(new ServiceDescriptor(typeof(IConnectionStringProvider), typeof(DefaultDbIsolationConnectionStringProvider),
                ServiceLifetime.Scoped));
        else
            services.TryAddScoped<IConnectionStringProvider, DefaultDbIsolationConnectionStringProvider>();

        MasaApp.TrySetServiceCollection(services);
    }

    public static IServiceCollection TryAddConfigure<TOptions>(
        this IServiceCollection services)
        where TOptions : class
    {
        services.AddOptions();
        var serviceProvider = services.BuildServiceProvider();
        IConfiguration? configuration = serviceProvider.GetService<IMasaConfiguration>()?.Local ??
            serviceProvider.GetService<IConfiguration>();

        if (configuration == null)
            return services;

        string name = Microsoft.Extensions.Options.Options.DefaultName;
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

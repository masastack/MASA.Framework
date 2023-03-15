// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

internal static class ServiceCollectionExtensions
{
    public static void AddIsolation(
        this IServiceCollection services,
        Action<IsolationBuilder> isolationBuilder,
        string sectionName = IsolationConstant.DEFAULT_SECTION_NAME)
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
            .AddConfigure<IsolationOptions>(sectionName)
            .AddTransient(typeof(IEventMiddleware<>), typeof(IsolationEventMiddleware<>));

        services.AddConnectionStringProvider();
        services.AddLocalMessageDbConnectionStringProvider();

        MasaApp.TrySetServiceCollection(services);
    }

    private static void AddConnectionStringProvider(this IServiceCollection services)
    {
        services.TryAddScoped<IIsolationConnectionStringProviderWrapper, DefaultIsolationConnectionStringProvider>();;
        var connectionStringProvider = ServiceDescriptor.Describe(typeof(IConnectionStringProvider), serviceProvider =>
        {
            var isolationOptions = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>();
            if (isolationOptions.Value.Enable)
                serviceProvider.GetRequiredService<IIsolationConnectionStringProviderWrapper>();

            return serviceProvider.GetRequiredService<IConnectionStringProviderWrapper>();
        }, ServiceLifetime.Scoped);
        if (services.Any(d => d.ServiceType == typeof(IConnectionStringProvider)))
            services.Replace(connectionStringProvider);
        else
            services.Add(connectionStringProvider);
    }

    private static void AddLocalMessageDbConnectionStringProvider(this IServiceCollection services)
    {
        services.TryAddScoped<IIsolationLocalMessageDbConnectionStringProviderWrapper,DefaultIsolationLocalMessageDbConnectionStringProvider>();
        var localMessageDbConnectionStringProvider = ServiceDescriptor.Describe(typeof(ILocalMessageDbConnectionStringProvider), serviceProvider =>
        {
            var isolationOptions = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>();
            if (isolationOptions.Value.Enable)
                serviceProvider.GetRequiredService<IIsolationLocalMessageDbConnectionStringProviderWrapper>();

            return serviceProvider.GetRequiredService<ILocalMessageDbConnectionStringProviderWrapper>();
        }, ServiceLifetime.Scoped);
        if (services.Any(d => d.ServiceType == typeof(ILocalMessageDbConnectionStringProvider)))
            services.Replace(localMessageDbConnectionStringProvider);
        else
            services.Add(localMessageDbConnectionStringProvider);
    }


    private sealed class IsolationProvider
    {
    }
}

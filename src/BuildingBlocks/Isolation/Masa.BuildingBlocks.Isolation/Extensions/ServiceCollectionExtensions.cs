// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests.Isolation")]

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddIsolation(
        this IServiceCollection services,
        Action<IsolationBuilder> isolationBuilder,
        Action<IsolationOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(isolationBuilder);

#if (NET8_0_OR_GREATER)
if (services.Any(service => service.IsKeyedService == false && service.ImplementationType == typeof(IsolationProvider)))
            return;
#else
        if (services.Any(service => service.ImplementationType == typeof(IsolationProvider)))
            return;
#endif

        services.AddSingleton<IsolationProvider>();

        var builder = new IsolationBuilder(services);
        isolationBuilder.Invoke(builder);

        if (!services.Any(service =>
                service.ServiceType == typeof(IMultiTenantContext) ||
                service.ServiceType == typeof(IMultiEnvironmentContext)))
            throw new NotSupportedException("Tenant isolation and environment isolation use at least one");

        services
            .Configure<IsolationOptions>(options =>
            {
                configure?.Invoke(options);

                if (options.SectionName.IsNullOrWhiteSpace())
                    options.SectionName = IsolationConstant.DEFAULT_SECTION_NAME;
            })
            .TryAddScoped<IIsolationConfigProvider, DefaultIsolationConfigProvider>();

        MasaApp.TrySetServiceCollection(services);
    }

#pragma warning disable S2094
    private sealed class IsolationProvider
    {
    }
#pragma warning restore S2094
}

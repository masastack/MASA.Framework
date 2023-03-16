﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests.Isolation")]
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

        services
            .AddHttpContextAccessor()
            .AddTransient(typeof(IEventMiddleware<>), typeof(IsolationEventMiddleware<>))
            .Configure<IsolationOptions>(options =>
            {
                options.SectionName = sectionName;
            })
            .TryAddScoped<IIsolationConfigurationProvider, DefaultIsolationConfigurationProvider>();

        MasaApp.TrySetServiceCollection(services);
    }

    private sealed class IsolationProvider
    {
    }
}

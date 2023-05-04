// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

public static class IsolationBuilderExtensions
{
    public const string DEFAULT_TENANT_NAME = "__tenant";

    public static IIsolationBuilder UseMultiTenant(this IIsolationBuilder isolationBuilder)
        => isolationBuilder.UseMultiTenant(DEFAULT_TENANT_NAME);

    public static IIsolationBuilder UseMultiTenant(this IIsolationBuilder isolationBuilder, string tenantName)
        => isolationBuilder.UseMultiTenant(tenantName, null);

    public static IIsolationBuilder UseMultiTenant(this IIsolationBuilder isolationBuilder, List<IParserProvider>? parserProviders)
        => isolationBuilder.UseMultiTenant(DEFAULT_TENANT_NAME, parserProviders);

    public static IIsolationBuilder UseMultiTenant(this IIsolationBuilder isolationBuilder, string tenantName, List<IParserProvider>? parserProviders)
    {
        if (isolationBuilder.Services.Any(service => service.ImplementationType == typeof(MultiTenantProvider)))
            return isolationBuilder;

        isolationBuilder.Services.AddSingleton<MultiTenantProvider>();

        isolationBuilder.Services
            .AddHttpContextAccessor()
            .AddTransient(typeof(IEventMiddleware<>), typeof(IsolationEventMiddleware<>))
            .AddScoped<IIsolationMiddleware>(serviceProvider => new MultiTenantMiddleware(serviceProvider, tenantName, parserProviders));
        isolationBuilder.Services.TryAddScoped<MultiTenantContext>();
        isolationBuilder.Services.TryAddScoped(typeof(IMultiTenantContext), serviceProvider => serviceProvider.GetRequiredService<MultiTenantContext>());
        isolationBuilder.Services.TryAddScoped(typeof(IMultiTenantSetter), serviceProvider => serviceProvider.GetRequiredService<MultiTenantContext>());

#pragma warning disable CS0618
        isolationBuilder.Services.TryAddScoped(typeof(ITenantContext), serviceProvider => serviceProvider.GetRequiredService<MultiTenantContext>());
        isolationBuilder.Services.TryAddScoped(typeof(ITenantSetter), serviceProvider => serviceProvider.GetRequiredService<MultiTenantContext>());
#pragma warning restore CS0618
        return isolationBuilder;
    }

    private sealed class MultiTenantProvider
    {
    }
}

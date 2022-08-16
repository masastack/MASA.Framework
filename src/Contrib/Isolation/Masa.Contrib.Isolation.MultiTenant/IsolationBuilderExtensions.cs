// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.MultiTenant;

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

        isolationBuilder.Services.AddScoped<IIsolationMiddleware>(serviceProvider => new MultiTenantMiddleware(serviceProvider, tenantName, parserProviders));
        isolationBuilder.Services.TryAddSingleton<IConvertProvider, ConvertProvider>();
        isolationBuilder.Services.TryAddScoped<TenantContext>();
        isolationBuilder.Services.TryAddScoped(typeof(ITenantContext), serviceProvider => serviceProvider.GetRequiredService<TenantContext>());
        isolationBuilder.Services.TryAddScoped(typeof(ITenantSetter), serviceProvider => serviceProvider.GetRequiredService<TenantContext>());
        return isolationBuilder;
    }

    private sealed class MultiTenantProvider
    {
    }
}

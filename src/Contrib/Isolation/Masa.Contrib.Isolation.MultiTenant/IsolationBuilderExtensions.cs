// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

public static class IsolationBuilderExtensions
{
    public static IIsolationBuilder UseMultiTenant(this IIsolationBuilder isolationBuilder)
        => isolationBuilder.UseMultiTenant(IsolationConstant.DEFAULT_MULTI_TENANT_NAME);

    public static IIsolationBuilder UseMultiTenant(this IIsolationBuilder isolationBuilder, string? multiTenantName)
        => isolationBuilder.UseMultiTenant(multiTenantName, null);

    public static IIsolationBuilder UseMultiTenant(this IIsolationBuilder isolationBuilder, List<IParserProvider>? parserProviders)
        => isolationBuilder.UseMultiTenant(null, parserProviders);

    public static IIsolationBuilder UseMultiTenant(
        this IIsolationBuilder isolationBuilder,
        string? multiTenantName,
        List<IParserProvider>? parserProviders)
    {

#if (NET8_0 || NET8_0_OR_GREATER)
        if (isolationBuilder.Services.Any(service => service.IsKeyedService == false && service.ImplementationType == typeof(MultiTenantProvider)))
            return isolationBuilder;
#else
        if (isolationBuilder.Services.Any(service => service.ImplementationType == typeof(MultiTenantProvider)))
            return isolationBuilder;
#endif

        isolationBuilder.Services.AddSingleton<MultiTenantProvider>();

        var actualMultiTenantName = !multiTenantName.IsNullOrWhiteSpace() ? multiTenantName : IsolationConstant.DEFAULT_MULTI_TENANT_NAME;
        isolationBuilder.Services.Configure<IsolationOptions>(options => options.MultiTenantIdName = actualMultiTenantName);

        isolationBuilder.Services
            .AddHttpContextAccessor()
            .AddTransient(typeof(IEventMiddleware<>), typeof(IsolationEventMiddleware<>))
            .AddScoped<IIsolationMiddleware>(serviceProvider => new MultiTenantMiddleware(serviceProvider, actualMultiTenantName, parserProviders));
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

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

public static class IsolationBuilderExtensions
{
    public static IIsolationBuilder UseMultiEnvironment(this IIsolationBuilder isolationBuilder)
        => isolationBuilder.UseMultiEnvironment(null);

    public static IIsolationBuilder UseMultiEnvironment(this IIsolationBuilder isolationBuilder, List<IParserProvider>? parserProviders)
        => isolationBuilder.UseMultiEnvironment(null, parserProviders);

    public static IIsolationBuilder UseMultiEnvironment(
        this IIsolationBuilder isolationBuilder,
        string? environmentName,
        List<IParserProvider>? parserProviders = null)
    {

#if (NET8_0_OR_GREATER)
        if (isolationBuilder.Services.Any(service => service.IsKeyedService == false && service.ImplementationType == typeof(EnvironmentProvider)))
            return isolationBuilder;
#else
        if (isolationBuilder.Services.Any(service => service.ImplementationType == typeof(EnvironmentProvider)))
            return isolationBuilder;
#endif

        isolationBuilder.Services.AddSingleton<EnvironmentProvider>();

        var actualEnvironmentName = !environmentName.IsNullOrWhiteSpace() ? environmentName : IsolationConstant.DEFAULT_MULTI_ENVIRONMENT_NAME;

        isolationBuilder.Services.Configure<IsolationOptions>(options => options.MultiEnvironmentName = actualEnvironmentName);

        isolationBuilder.Services
            .AddHttpContextAccessor()
            .AddTransient(typeof(IEventMiddleware<>), typeof(IsolationEventMiddleware<>))
            .AddScoped<IIsolationMiddleware>(serviceProvider =>
                new MultiEnvironmentMiddleware(
                    serviceProvider,
                    actualEnvironmentName,
                    parserProviders,
                    serviceProvider.GetService<IOptions<MasaAppConfigureOptions>>()));
        isolationBuilder.Services.TryAddScoped<MultiEnvironmentContext>();
        isolationBuilder.Services.TryAddScoped(typeof(IMultiEnvironmentContext), serviceProvider => serviceProvider.GetRequiredService<MultiEnvironmentContext>());
        isolationBuilder.Services.TryAddScoped(typeof(IMultiEnvironmentSetter), serviceProvider => serviceProvider.GetRequiredService<MultiEnvironmentContext>());

#pragma warning disable CS0618
        isolationBuilder.Services.TryAddScoped(typeof(IEnvironmentContext), serviceProvider => serviceProvider.GetRequiredService<MultiEnvironmentContext>());
        isolationBuilder.Services.TryAddScoped(typeof(IEnvironmentSetter), serviceProvider => serviceProvider.GetRequiredService<MultiEnvironmentContext>());
#pragma warning restore CS0618
        return isolationBuilder;
    }

    private sealed class EnvironmentProvider
    {
    }
}

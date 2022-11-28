// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.MultiEnvironment;

public static class IsolationBuilderExtensions
{
    public static IIsolationBuilder UseMultiEnvironment(this IIsolationBuilder isolationBuilder)
        => isolationBuilder.UseMultiEnvironment(null);

    public static IIsolationBuilder UseMultiEnvironment(this IIsolationBuilder isolationBuilder, List<IParserProvider>? parserProviders)
        => isolationBuilder.UseMultiEnvironment(null, parserProviders);

    public static IIsolationBuilder UseMultiEnvironment(this IIsolationBuilder isolationBuilder, string? environmentName, List<IParserProvider>? parserProviders = null)
    {
        if (isolationBuilder.Services.Any(service => service.ImplementationType == typeof(EnvironmentProvider)))
            return isolationBuilder;

        isolationBuilder.Services.AddSingleton<EnvironmentProvider>();

        isolationBuilder.Services.AddScoped<IIsolationMiddleware>(serviceProvider =>
            new MultiEnvironmentMiddleware(
                serviceProvider,
                environmentName,
                parserProviders,
                serviceProvider.GetService<IOptions<MasaAppConfigureOptions>>()));
        isolationBuilder.Services.TryAddScoped<MultiEnvironmentContext>();
        isolationBuilder.Services.TryAddScoped(typeof(IMultiEnvironmentContext),
            serviceProvider => serviceProvider.GetRequiredService<MultiEnvironmentContext>());
        isolationBuilder.Services.TryAddScoped(typeof(IMultiEnvironmentSetter),
            serviceProvider => serviceProvider.GetRequiredService<MultiEnvironmentContext>());
        return isolationBuilder;
    }

    private sealed class EnvironmentProvider
    {
    }
}

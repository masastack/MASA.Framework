// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.MultiEnvironment;

public static class IsolationBuilderExtensions
{
    public const string DEFAULT_ENVIRONMENT_NAME = "ASPNETCORE_ENVIRONMENT";

    public static IIsolationBuilder UseMultiEnvironment(this IIsolationBuilder isolationBuilder)
        => isolationBuilder.UseMultiEnvironment(DEFAULT_ENVIRONMENT_NAME);

    public static IIsolationBuilder UseMultiEnvironment(this IIsolationBuilder isolationBuilder, List<IParserProvider>? parserProviders)
        => isolationBuilder.UseMultiEnvironment(DEFAULT_ENVIRONMENT_NAME, parserProviders);

    public static IIsolationBuilder UseMultiEnvironment(this IIsolationBuilder isolationBuilder, string environmentName, List<IParserProvider>? parserProviders = null)
    {
        if (isolationBuilder.Services.Any(service => service.ImplementationType == typeof(EnvironmentProvider)))
            return isolationBuilder;

        isolationBuilder.Services.AddSingleton<EnvironmentProvider>();

        isolationBuilder.Services.AddScoped<IIsolationMiddleware>(serviceProvider => new MultiEnvironmentMiddleware(serviceProvider, environmentName, parserProviders));
        isolationBuilder.Services.TryAddScoped<EnvironmentContext>();
        isolationBuilder.Services.TryAddScoped(typeof(IEnvironmentContext), serviceProvider => serviceProvider.GetRequiredService<EnvironmentContext>());
        isolationBuilder.Services.TryAddScoped(typeof(IEnvironmentSetter), serviceProvider => serviceProvider.GetRequiredService<EnvironmentContext>());
        return isolationBuilder;
    }

    private sealed class EnvironmentProvider
    {
    }
}

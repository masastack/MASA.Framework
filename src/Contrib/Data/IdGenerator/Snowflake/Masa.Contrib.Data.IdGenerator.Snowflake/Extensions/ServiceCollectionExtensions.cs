// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSnowflake(this IServiceCollection services)
        => services.AddIdGenerator(options => options.UseSnowflakeGenerator());

    public static IServiceCollection AddSnowflake(this IServiceCollection services, Action<SnowflakeGeneratorOptions>? action)
        => services.AddIdGenerator(options => options.UseSnowflakeGenerator(action));
}

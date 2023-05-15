// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSequentialGuidGenerator(this IServiceCollection services)
        => services.AddIdGenerator(options => options.UseSequentialGuidGenerator());

    public static IServiceCollection AddSequentialGuidGenerator(this IServiceCollection services, SequentialGuidType guidType)
        => services.AddIdGenerator(options => options.UseSequentialGuidGenerator(guidType));
}

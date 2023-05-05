// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Data.IdGenerator.NormalGuid")]
[assembly: InternalsVisibleTo("Masa.Contrib.Data.IdGenerator.SequentialGuid")]
[assembly: InternalsVisibleTo("Masa.Contrib.Data.IdGenerator.Snowflake")]

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdGenerator(this IServiceCollection services, Action<IdGeneratorOptions> configure)
    {
        services.TryAddSingleton<IIdGeneratorFactory, DefaultIdGeneratorFactory>();

        var idGeneratorOptions = new IdGeneratorOptions(services);
        configure.Invoke(idGeneratorOptions);
        MasaApp.TrySetServiceCollection(services);
        return services;
    }
}

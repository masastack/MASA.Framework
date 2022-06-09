// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.NormalGuid;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSimpleGuidGenerator(this IServiceCollection services)
    {
        services.TryAddSingleton<IGuidGenerator, NormalGuidGenerator>();
        services.TryAddSingleton<IIdGenerator<Guid, Guid>, IGuidGenerator>();
        IdGeneratorFactory.SetGuidGenerator(services.BuildServiceProvider().GetRequiredService<IGuidGenerator>());
        return services;
    }
}

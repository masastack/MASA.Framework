// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.SequentialGuid;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSequentialGuidGenerator(this IServiceCollection services)
        => services.AddSequentialGuidGenerator(SequentialGuidType.SequentialAtEnd);

    public static IServiceCollection AddSequentialGuidGenerator(this IServiceCollection services, SequentialGuidType guidType)
    {
        services.TryAddSingleton<ISequentialGuidGenerator>(_ => new SequentialGuidGenerator(guidType));
        services.TryAddSingleton<IIdGenerator<System.SequentialGuid, Guid>>(serviceProvider
            => serviceProvider.GetRequiredService<ISequentialGuidGenerator>());
        IdGeneratorFactory.SetSequentialGuidGenerator(services.BuildServiceProvider().GetRequiredService<ISequentialGuidGenerator>());
        return services;
    }
}

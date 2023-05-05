﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

public static class IdGeneratorOptionsExtensions
{
    public static void UseSimpleGuidGenerator(this IdGeneratorOptions options)
    {
        if (options.Services.Any(service => service.ImplementationType == typeof(SimpleGuidGeneratorProvider))) return;

        options.Services.AddSingleton<SimpleGuidGeneratorProvider>();

        options.Services.AddSingleton<IGuidGenerator, NormalGuidGenerator>();
        options.Services.TryAddSingleton<IIdGenerator<Guid>>(serviceProvider => serviceProvider.GetRequiredService<IGuidGenerator>());
        options.Services.TryAddSingleton<IIdGenerator>(serviceProvider => serviceProvider.GetRequiredService<IGuidGenerator>());
    }

    #pragma warning disable S2094
    private sealed class SimpleGuidGeneratorProvider
    {
    }
    #pragma warning restore S2094
}

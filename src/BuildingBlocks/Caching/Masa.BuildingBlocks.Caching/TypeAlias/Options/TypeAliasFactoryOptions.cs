// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Caching;

public class TypeAliasFactoryOptions : MasaFactoryOptions<TypeAliasRelationOptions>
{
    public void TryAdd(string name)
    {
        if (Options.Any(options => options.Name == name))
            return;

        var typeAliasRelationOptions = new TypeAliasRelationOptions(
            name,
            serviceProvider
                => new DefaultTypeAliasProvider(serviceProvider.GetService<IOptionsFactory<CacheKeyAliasOptions>>()?.Create(name))
        );
        Options.Add(typeAliasRelationOptions);
    }
}

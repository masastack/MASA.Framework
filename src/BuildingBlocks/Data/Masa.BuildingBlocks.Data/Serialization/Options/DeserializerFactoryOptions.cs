// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

public class DeserializerFactoryOptions : MasaFactoryOptions<MasaRelationOptions<IDeserializer>>
{
    public void TryAdd(string name, Func<IServiceProvider, IDeserializer> func)
    {
        if (Options.Any(opt => opt.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException(
                $"The deserializer name already exists, please change the name, the repeat name is [{name}]");

        Options.Add(new MasaRelationOptions<IDeserializer>(name, func));
    }
}

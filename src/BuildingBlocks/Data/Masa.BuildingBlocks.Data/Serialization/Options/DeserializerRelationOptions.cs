// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class DeserializerRelationOptions : MasaRelationOptions<IDeserializer>
{
    public DeserializerRelationOptions(string name, Func<IServiceProvider, IDeserializer> func)
        : base(name)
    {
        Func = func;
    }
}

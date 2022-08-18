// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class SerializerRelationOptions : MasaRelationOptions<ISerializer>
{
    public SerializerRelationOptions(string name, Func<IServiceProvider, ISerializer> func)
        : base(name)
    {
        Func = func;
    }
}

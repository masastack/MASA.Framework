// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller.Options;

public class CallerRelationOptions : MasaRelationOptions<ICaller>
{
    public CallerRelationOptions(string name, Func<IServiceProvider, ICaller> func) : base(name)
    {
        Name = name;
        Func = func;
    }
}

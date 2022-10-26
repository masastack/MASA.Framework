// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.RulesEngine;

public class RulesEngineRelationOptions : MasaRelationOptions<IRulesEngineClient>
{
    public RulesEngineRelationOptions(string name, Func<IServiceProvider, IRulesEngineClient> func)
        : base(name)
    {
        Func = func;
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.RulesEngine;

public class RulesEngineFactoryOptions : MasaFactoryOptions<RulesEngineRelationOptions>
{
    public void TryAddRulesEngine(string name, Func<IServiceProvider, IRulesEngineClient> func)
    {
        if (Options.Any(opt => opt.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return;

        Options.Add(new RulesEngineRelationOptions(name, func));
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.RulesEngine;

public static class RulesEngineOptionsBuilderExtensions
{
    public static void UseCustomRulesEngine(
        this RulesEngineOptionsBuilder rulesEngineOptionsBuilder,
        Func<IServiceProvider, IRulesEngineClient> func)
    {
        rulesEngineOptionsBuilder.Services.Configure<RulesEngineFactoryOptions>(options =>
        {
            options.TryAddRulesEngine(rulesEngineOptionsBuilder.Name, func.Invoke);
        });
    }
}

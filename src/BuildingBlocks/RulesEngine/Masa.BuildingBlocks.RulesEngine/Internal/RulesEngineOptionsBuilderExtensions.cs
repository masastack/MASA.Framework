// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.RulesEngine.MicrosoftRulesEngine")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.RulesEngine;

internal static class RulesEngineOptionsBuilderExtensions
{
    public static void AddRulesEngine(
        this RulesEngineOptionsBuilder rulesEngineOptionsBuilder,
        Func<IServiceProvider, IRulesEngineClient> func)
    {
        rulesEngineOptionsBuilder.Services.Configure<RulesEngineFactoryOptions>(options =>
        {
            options.TryAddRulesEngine(rulesEngineOptionsBuilder.Name, func.Invoke);
        });
    }
}

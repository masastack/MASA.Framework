// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

extern alias MicrosoftRulesEngine;
using global::Masa.Contrib.RulesEngine.MicrosoftRulesEngine;
using MicrosoftRulesEngine::RulesEngine.Models;

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.RulesEngine;

public static class RulesEngineOptionsExtensions
{
    public static RulesEngineOptionsBuilder UseMicrosoftRulesEngine(
        this RulesEngineOptionsBuilder rulesEngineOptionsBuilder,
        ReSettings? reSettings = null)
    {
        rulesEngineOptionsBuilder.UseCustomRulesEngine(serviceProvider
            => new RulesEngineClient(reSettings, serviceProvider.GetService<ILogger<RulesEngineClient>>()));
        return rulesEngineOptionsBuilder;
    }

    public static RulesEngineOptionsBuilder UseMicrosoftRulesEngine(
        this RulesEngineOptionsBuilder rulesEngineOptionsBuilder,
        Action<ReSettings>? configure)
    {
        rulesEngineOptionsBuilder.UseCustomRulesEngine(serviceProvider =>
        {
            var reSettings = new ReSettings();
            configure?.Invoke(reSettings);
            return new RulesEngineClient(reSettings, serviceProvider.GetService<ILogger<RulesEngineClient>>());
        });
        return rulesEngineOptionsBuilder;
    }
}

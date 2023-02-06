// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

extern alias MicrosoftRulesEngine;
using global::Masa.Contrib.RulesEngine.MicrosoftRulesEngine;
using MicrosoftRulesEngine::RulesEngine.Models;

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.RulesEngine;

public static class RulesEngineOptionsExtensions
{
    public static RulesEngineOptions UseMicrosoftRulesEngine(this RulesEngineOptions rulesEngineOptions)
        => rulesEngineOptions.UseMicrosoftRulesEngine(Options.DefaultName);

    public static RulesEngineOptions UseMicrosoftRulesEngine(this RulesEngineOptions rulesEngineOptions, ReSettings? reSettings)
        => rulesEngineOptions.UseMicrosoftRulesEngine(Options.DefaultName, reSettings);

    public static RulesEngineOptions UseMicrosoftRulesEngine(
        this RulesEngineOptions rulesEngineOptions,
        string name,
        ReSettings? reSettings = null)
    {
        rulesEngineOptions.Services.Configure<RulesEngineFactoryOptions>(options =>
        {
            options.TryAddRulesEngine(name,
                serviceProvider => new RulesEngineClient(reSettings, serviceProvider.GetService<ILogger<RulesEngineClient>>())
            );
        });
        return rulesEngineOptions;
    }
}

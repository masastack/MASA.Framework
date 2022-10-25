// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

extern alias MicrosoftRulesEngine;
using global::Masa.Contrib.RulesEngine.MicrosoftRulesEngine;
using MicrosoftRulesEngine::RulesEngine.Models;

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class RulesEngineOptionsExtensions
{
    public static RulesEngineOptions UseRulesEngine(this RulesEngineOptions rulesEngineOptions)
        => rulesEngineOptions.UseRulesEngine(Options.Options.DefaultName);

    public static RulesEngineOptions UseRulesEngine(this RulesEngineOptions rulesEngineOptions, ReSettings? reSettings)
        => rulesEngineOptions.UseRulesEngine(Options.Options.DefaultName, reSettings);

    public static RulesEngineOptions UseRulesEngine(
        this RulesEngineOptions rulesEngineOptions,
        string name,
        ReSettings? reSettings = null)
    {
        rulesEngineOptions.Services.Configure<RulesEngineFactoryOptions>(name, options =>
        {
            options.TryAddRulesEngine(name,
                serviceProvider => new RulesEngineClient(reSettings)
            );
        });
        return rulesEngineOptions;
    }
}

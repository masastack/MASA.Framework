// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.RulesEngine;

internal class ScopedRulesEngine
{
    public IRulesEngineClient RulesEngineClient { get; }

    public ScopedRulesEngine(IRulesEngineClient rulesEngineClient)
    {
        RulesEngineClient = rulesEngineClient;
    }
}

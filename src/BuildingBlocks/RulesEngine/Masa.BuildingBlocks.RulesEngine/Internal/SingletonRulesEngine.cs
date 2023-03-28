// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.RulesEngine;

internal class SingletonRulesEngine
{
    public IRulesEngineClient RulesEngineClient { get; }

    public SingletonRulesEngine(IRulesEngineClient rulesEngineClient)
    {
        RulesEngineClient = rulesEngineClient;
    }
}

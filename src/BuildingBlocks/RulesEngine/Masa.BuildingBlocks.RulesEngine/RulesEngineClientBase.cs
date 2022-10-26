// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.RulesEngine;

public abstract class RulesEngineClientBase : IRulesEngineClient
{
    public abstract VerifyResponse Verify(string ruleRaw);

    public abstract List<ExecuteResponse> Execute<TRequest>(string ruleRaw, TRequest data);

    public abstract Task<List<ExecuteResponse>> ExecuteAsync<TRequest>(string ruleRaw, TRequest data);
}

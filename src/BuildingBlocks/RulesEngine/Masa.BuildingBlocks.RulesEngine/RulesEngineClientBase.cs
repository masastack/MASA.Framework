// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.RulesEngine;

public abstract class RulesEngineClientBase : IRulesEngineClient
{
    public abstract bool Execute<TRequest>(string ruleJson, TRequest data);

    public virtual List<(TRequest Data, bool Result)> Execute<TRequest>(string ruleJson, TRequest[] datum)
        => Execute(ruleJson, (IEnumerable<TRequest>)datum);

    public abstract List<(TRequest Data, bool Result)> Execute<TRequest>(string ruleJson, IEnumerable<TRequest> datum);

    public abstract Task<bool> ExecuteAsync<TRequest>(string ruleJson, TRequest data);

    public virtual Task<List<(TRequest Data, bool Result)>> ExecuteAsync<TRequest>(string ruleJson, TRequest[] datum)
        => ExecuteAsync(ruleJson, (IEnumerable<TRequest>)datum);

    public abstract Task<List<(TRequest Data, bool Result)>> ExecuteAsync<TRequest>(string ruleJson, IEnumerable<TRequest> datum);
}

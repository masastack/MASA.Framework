// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.RulesEngine;

public class RulesEngineClient : IRulesEngineClient
{
    private readonly IJsonDeserializer _jsonDeserializer;

    public RulesEngineClient(IJsonDeserializer jsonDeserializer)
    {
        _jsonDeserializer = jsonDeserializer;
    }

    public bool Execute<TRequest>(string ruleJson, TRequest data)
    {
        var workFlow = _jsonDeserializer.Deserialize<Workflow>(ruleJson);
        return false;
    }

    public Task<bool> ExecuteAsync<TRequest>(string ruleJson, TRequest data)
    {
        throw new NotImplementedException();
    }
}

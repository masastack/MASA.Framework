// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.RulesEngine.MicrosoftRulesEngine;

extern alias MicrosoftRulesEngine;
using MicrosoftRulesEngine::RulesEngine;
using MicrosoftRulesEngine::RulesEngine.Models;

public class RulesEngineClient : RulesEngineClientBase
{
    private readonly RulesEngine _rulesEngine;

    public RulesEngineClient(ReSettings? reSettings = null)
    {
        _rulesEngine = new RulesEngine(Array.Empty<Workflow>(), reSettings);
    }

    public override bool Execute<TRequest>(string ruleJson, TRequest data)
        => ExecuteAsync(ruleJson, data).ConfigureAwait(false).GetAwaiter().GetResult();

    public override List<(TRequest Data, bool Result)> Execute<TRequest>(string ruleJson, IEnumerable<TRequest> datum)
        => ExecuteAsync(ruleJson, datum).ConfigureAwait(false).GetAwaiter().GetResult();

    public override async Task<bool> ExecuteAsync<TRequest>(string ruleJson, TRequest data)
    {
        CheckAndAddRule(ruleJson, out Workflow? workflow);

        var ruleResultTrees = await _rulesEngine.ExecuteAllRulesAsync(workflow!.WorkflowName, data);
        return ruleResultTrees.Any(x => x.IsSuccess);
    }

    public override async Task<List<(TRequest Data, bool Result)>> ExecuteAsync<TRequest>(string ruleJson, IEnumerable<TRequest> datum)
    {
        CheckAndAddRule(ruleJson, out Workflow? workflow);

        List<(TRequest, bool)> list = new();
        foreach (var data in datum)
        {
            var ruleResultTrees = await _rulesEngine.ExecuteAllRulesAsync(workflow!.WorkflowName, data);
            list.Add((data, ruleResultTrees.Any(x => x.IsSuccess)));
        }
        return list;
    }

    private void CheckAndAddRule(string ruleJson, out Workflow? workflow)
    {
        _rulesEngine.ClearWorkflows();
        workflow = JsonConvert.DeserializeObject<Workflow>(ruleJson);
        ArgumentNullException.ThrowIfNull(workflow, nameof(ruleJson));
        _rulesEngine.AddWorkflow(workflow);
    }
}

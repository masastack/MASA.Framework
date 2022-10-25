// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.RulesEngine.MicrosoftRulesEngine;

extern alias MicrosoftRulesEngine;
using MicrosoftRulesEngine::RulesEngine;
using MicrosoftRulesEngine::RulesEngine.Models;

public class RulesEngineClient : RulesEngineClientBase
{
    private readonly RulesEngine _rulesEngine;
    private readonly ILogger<RulesEngineClient>? _logger;

    public RulesEngineClient(ReSettings? reSettings = null, ILogger<RulesEngineClient>? logger = null)
    {
        _rulesEngine = new RulesEngine(Array.Empty<Workflow>(), reSettings);
        _logger = logger;
    }

    public override VerifyResponse Verify(string ruleRaw)
    {
        try
        {
            if (JsonConvert.DeserializeObject<Workflow>(ruleRaw) == null)
            {
                return new VerifyResponse(false, "illegal rules");
            }
            return new VerifyResponse(true);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "illegal rules on MicrosoftRulesEngine");
            return new VerifyResponse(false, ex.Message);
        }
    }

    public override List<ExecuteResponse> Execute<TRequest>(string ruleRaw, TRequest data)
        => ExecuteAsync(ruleRaw, data).ConfigureAwait(false).GetAwaiter().GetResult();

    public override async Task<List<ExecuteResponse>> ExecuteAsync<TRequest>(string ruleRaw, TRequest data)
    {
        CheckAndAddRule(ruleRaw, out Workflow? workflow);

        var ruleResultTrees = await _rulesEngine.ExecuteAllRulesAsync(workflow!.WorkflowName, data);
        return ConvertToExecuteResponse(ruleResultTrees);
    }

    private void CheckAndAddRule(string ruleRaw, out Workflow? workflow)
    {
        _rulesEngine.ClearWorkflows();
        workflow = JsonConvert.DeserializeObject<Workflow>(ruleRaw);
        ArgumentNullException.ThrowIfNull(workflow, nameof(ruleRaw));
        if (string.IsNullOrWhiteSpace(workflow.WorkflowName))
            workflow.WorkflowName = Guid.NewGuid().ToString();
        _rulesEngine.AddWorkflow(workflow);
    }

    private static List<ExecuteResponse> ConvertToExecuteResponse(List<RuleResultTree> resultTrees)
    {
        return resultTrees.Select(result => new ExecuteResponse(result.Rule.RuleName, result.IsSuccess, result.ExceptionMessage)
        {
            SuccessEvent = result.Rule.SuccessEvent,
            ActionResult = new ActionResponse()
            {
                Exception = result.ActionResult.Exception,
                Output = result.ActionResult.Output
            }
        }).ToList();
    }
}

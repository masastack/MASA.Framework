// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.RulesEngine;

public class ExecuteResponse : ResponseBase
{
    public string RuleName { get; set; }

    public string SuccessEvent { get; set; }

    public ActionResponse ActionResult { get; set; }

    public ExecuteResponse(string ruleName, bool isValid, string message = "")
        : base(isValid, message)
    {
        RuleName = ruleName;
    }
}

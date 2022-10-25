// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.RulesEngine;

public interface IRulesEngineClient
{
    /// <summary>
    /// Check if the rule format is correct
    /// </summary>
    /// <param name="ruleRaw"></param>
    /// <returns></returns>
    VerifyResponse Verify(string ruleRaw);

    /// <summary>
    /// Obtain whether the match is successful according to the rules and input parameters
    /// </summary>
    /// <param name="ruleRaw"></param>
    /// <param name="data">Input data</param>
    /// <typeparam name="TRequest"></typeparam>
    /// <returns>Whether the match was successful</returns>
    List<ExecuteResponse> Execute<TRequest>(string ruleRaw, TRequest data);

    /// <summary>
    /// Obtain whether the match is successful according to the rules and input parameters
    /// </summary>
    /// <param name="ruleRaw">Rule Json</param>
    /// <param name="data">Input data</param>
    /// <typeparam name="TRequest"></typeparam>
    /// <returns>Whether the match was successful</returns>
    Task<List<ExecuteResponse>> ExecuteAsync<TRequest>(string ruleRaw, TRequest data);
}

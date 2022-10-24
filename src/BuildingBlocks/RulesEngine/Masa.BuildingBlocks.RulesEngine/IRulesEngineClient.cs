// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.RulesEngine;

public interface IRulesEngineClient
{
    /// <summary>
    /// Obtain whether the match is successful according to the rules and input parameters
    /// </summary>
    /// <param name="ruleJson">Rule Json</param>
    /// <param name="data">Input data</param>
    /// <typeparam name="TRequest"></typeparam>
    /// <returns>Whether the match was successful</returns>
    bool Execute<TRequest>(string ruleJson, TRequest data);

    /// <summary>
    /// Obtain whether the match is successful according to the rules and input parameters
    /// </summary>
    /// <param name="ruleJson">Rule Json</param>
    /// <param name="data">Input data</param>
    /// <typeparam name="TRequest"></typeparam>
    /// <returns>Whether the match was successful</returns>
    Task<bool> ExecuteAsync<TRequest>(string ruleJson, TRequest data);
}

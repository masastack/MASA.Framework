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
    /// Obtain parameters and corresponding execution sets according to rules and input parameter sets
    /// </summary>
    /// <param name="ruleJson">Rule Json</param>
    /// <param name="datum">Set of input parameters</param>
    /// <typeparam name="TRequest"></typeparam>
    /// <returns>Input parameters and execution result set</returns>
    List<(TRequest Data, bool Result)> Execute<TRequest>(string ruleJson, TRequest[] datum);

    /// <summary>
    /// Obtain parameters and corresponding execution sets according to rules and input parameter sets
    /// </summary>
    /// <param name="ruleJson">Rule Json</param>
    /// <param name="datum">Set of input parameters</param>
    /// <typeparam name="TRequest"></typeparam>
    /// <returns>Input parameters and execution result set</returns>
    List<(TRequest Data, bool Result)> Execute<TRequest>(string ruleJson, IEnumerable<TRequest> datum);

    /// <summary>
    /// Obtain whether the match is successful according to the rules and input parameters
    /// </summary>
    /// <param name="ruleJson">Rule Json</param>
    /// <param name="data">Input data</param>
    /// <typeparam name="TRequest"></typeparam>
    /// <returns>Whether the match was successful</returns>
    Task<bool> ExecuteAsync<TRequest>(string ruleJson, TRequest data);

    /// <summary>
    /// Obtain parameters and corresponding execution sets according to rules and input parameter sets
    /// </summary>
    /// <param name="ruleJson">Rule Json</param>
    /// <param name="datum">Set of input parameters</param>
    /// <typeparam name="TRequest"></typeparam>
    /// <returns>Input parameters and execution result set</returns>
    Task<List<(TRequest Data, bool Result)>> ExecuteAsync<TRequest>(string ruleJson, TRequest[] datum);

    /// <summary>
    /// Obtain parameters and corresponding execution sets according to rules and input parameter sets
    /// </summary>
    /// <param name="ruleJson">Rule Json</param>
    /// <param name="datum">Set of input parameters</param>
    /// <typeparam name="TRequest"></typeparam>
    /// <returns>Input parameters and execution result set</returns>
    Task<List<(TRequest Data, bool Result)>> ExecuteAsync<TRequest>(string ruleJson, IEnumerable<TRequest> datum);
}

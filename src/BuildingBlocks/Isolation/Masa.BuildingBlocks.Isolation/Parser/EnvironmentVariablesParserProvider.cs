// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Isolation.Parser;

public class EnvironmentVariablesParserProvider : IParserProvider
{
    public string Name { get; } = "EnvironmentVariables";

    public Task<bool> ResolveAsync(HttpContext? httpContext, string key, Action<string> action)
    {
        string? value = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrEmpty(value))
        {
            action.Invoke(value);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}

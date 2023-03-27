// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.Parser;

public class HttpContextItemParserProvider : IParserProvider
{
    public string Name => "Items";

    public Task<bool> ResolveAsync(HttpContext? httpContext, string key, Action<string> action)
    {
        if (httpContext?.Items.ContainsKey(key) ?? false)
        {
            var value = httpContext.Items[key]?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                action.Invoke(value);
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }
}

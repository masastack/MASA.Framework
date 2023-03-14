// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.Parser;

public class HeaderParserProvider : IParserProvider
{
    public string Name => "Header";

    public Task<bool> ResolveAsync(HttpContext? httpContext, string key, Action<string> action)
    {
        if (httpContext?.Request.Headers.ContainsKey(key) ?? false)
        {
            var value = httpContext.Request.Headers[key].ToString();
            if (!string.IsNullOrEmpty(value))
            {
                action.Invoke(value);
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }
}

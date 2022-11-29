// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Isolation.Parser;

public class FormParserProvider : IParserProvider
{
    public string Name => "Form";

    public Task<bool> ResolveAsync(HttpContext? httpContext, string key, Action<string> action)
    {
        if (!(httpContext?.Request.HasFormContentType ?? false))
            return Task.FromResult(false);

        if (httpContext?.Request.Form.ContainsKey(key) ?? false)
        {
            var value = httpContext.Request.Form[key].ToString();
            if (!string.IsNullOrEmpty(value))
            {
                action.Invoke(value);
                return Task.FromResult(true);
            }
        }
        return Task.FromResult(false);
    }
}

﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Isolation.Parser;

public class CookieParserProvider : IParserProvider
{
    public string Name => "Cookie";

    public Task<bool> ResolveAsync(IServiceProvider serviceProvider, string key, Action<string> action)
    {
        var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
        if (httpContext?.Request.Cookies.ContainsKey(key) ?? false)
        {
            var value = httpContext.Request.Cookies[key];
            if (!string.IsNullOrEmpty(value))
            {
                action.Invoke(value);
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18N.BlazorWebAssembly.Extensions;

public static class WebAssemblyHostBuilderExtensions
{
    public static async Task<WebAssemblyHostBuilder> AddI18NForBlazorWebAssemblyAsync(
        this WebAssemblyHostBuilder builder,
        Action<I18NOptions>? action = null)
    {
        await builder.Services.AddI18NForBlazorWebAssemblyAsync(builder.HostEnvironment.BaseAddress, action);
        return builder;
    }
}

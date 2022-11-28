// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Isolation.MultiEnvironment;

public class MasaAppConfigureParserProvider : IParserProvider
{
    public string Name => "MasaAppConfigure";

    public Task<bool> ResolveAsync(IServiceProvider serviceProvider, string key, Action<string> action)
    {
        var environment = serviceProvider.GetService<IOptions<MasaAppConfigureOptions>>()?.Value?.Environment;
        if (!string.IsNullOrWhiteSpace(environment))
        {
            action.Invoke(environment);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}

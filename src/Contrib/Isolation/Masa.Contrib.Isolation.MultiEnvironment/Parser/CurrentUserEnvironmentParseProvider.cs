// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Isolation.MultiEnvironment;

public class CurrentUserEnvironmentParseProvider : IParserProvider
{
    public string Name => "CurrentUser";

    public Task<bool> ResolveAsync(IServiceProvider serviceProvider, string key, Action<string> action)
    {
        var multiEnvironmentUserContext = serviceProvider.GetService<IMultiEnvironmentUserContext>();
        var environment = multiEnvironmentUserContext?.Environment;
        if (!string.IsNullOrWhiteSpace(environment))
        {
            action.Invoke(environment);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}

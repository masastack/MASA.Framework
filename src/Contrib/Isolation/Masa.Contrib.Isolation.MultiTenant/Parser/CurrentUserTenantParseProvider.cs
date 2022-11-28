// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Isolation;

public class CurrentUserTenantParseProvider : IParserProvider
{
    public string Name => "CurrentUser";

    public Task<bool> ResolveAsync(IServiceProvider serviceProvider, string key, Action<string> action)
    {
        var multiTenantUserContext = serviceProvider.GetService<IMultiTenantUserContext>();
        var tenantId = multiTenantUserContext?.TenantId;
        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            action.Invoke(tenantId);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}

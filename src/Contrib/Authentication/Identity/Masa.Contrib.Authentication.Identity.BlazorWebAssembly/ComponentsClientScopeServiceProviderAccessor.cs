// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Identity.BlazorWebAssembly;

public class ComponentsClientScopeServiceProviderAccessor : IClientScopeServiceProviderAccessor
{
    public IServiceProvider ServiceProvider { get; set; }
}

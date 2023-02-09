// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.OpenIdConnect;

public class AuthenticationOptions
{
    public IServiceCollection Services { get; }

    public AuthenticationOptions(IServiceCollection services) => Services = services;
}

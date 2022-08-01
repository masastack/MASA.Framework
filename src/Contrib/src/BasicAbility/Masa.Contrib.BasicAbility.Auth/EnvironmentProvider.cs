// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth;

public class EnvironmentProvider : IEnvironmentProvider
{
    readonly IMultiEnvironmentUserContext _multiEnvironmentUserContext;

    public EnvironmentProvider(IMultiEnvironmentUserContext multiEnvironmentUserContext)
    {
        _multiEnvironmentUserContext = multiEnvironmentUserContext;
    }

    public string GetEnvironment()
    {
        return _multiEnvironmentUserContext.Environment ?? "";
    }
}

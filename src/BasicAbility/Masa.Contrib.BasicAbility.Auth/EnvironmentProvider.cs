// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth;

public class EnvironmentProvider : IEnvironmentProvider
{
    public string GetEnvironment()
    {
        return "development";
    }
}

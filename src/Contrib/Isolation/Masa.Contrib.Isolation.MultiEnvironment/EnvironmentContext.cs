// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.MultiEnvironment;

public class EnvironmentContext : IEnvironmentContext, IEnvironmentSetter
{
    public string CurrentEnvironment { get; private set; } = string.Empty;

    public void SetEnvironment(string environment) => CurrentEnvironment = environment;
}

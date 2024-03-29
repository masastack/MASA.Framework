// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

public interface IMultiEnvironmentSetter
{
    void SetEnvironment(string environment);
}

[Obsolete("Use IMultiEnvironmentSetter instead")]
public interface IEnvironmentSetter
{
    void SetEnvironment(string environment);
}

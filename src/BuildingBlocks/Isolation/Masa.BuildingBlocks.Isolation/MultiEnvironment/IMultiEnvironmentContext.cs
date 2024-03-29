// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

public interface IMultiEnvironmentContext
{
    string CurrentEnvironment { get; }
}

[Obsolete("Use IMultiEnvironmentContext instead")]
public interface IEnvironmentContext
{
    string CurrentEnvironment { get; }
}

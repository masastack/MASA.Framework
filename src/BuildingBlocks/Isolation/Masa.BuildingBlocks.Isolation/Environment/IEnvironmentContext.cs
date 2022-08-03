// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Isolation.Environment;

public interface IEnvironmentContext
{
    string CurrentEnvironment { get; }
}

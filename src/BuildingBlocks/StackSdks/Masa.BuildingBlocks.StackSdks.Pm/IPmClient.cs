// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Pm;

public interface IPmClient
{
    public IEnvironmentService EnvironmentService { get; init; }

    public IClusterService ClusterService { get; init; }

    public IProjectService ProjectService { get; init; }

    public IAppService AppService { get; init; }
}

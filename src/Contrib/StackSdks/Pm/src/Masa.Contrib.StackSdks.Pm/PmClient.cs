// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.StackSdks.Pm;
using Masa.BuildingBlocks.StackSdks.Pm.Service;
using Masa.Contrib.StackSdks.Pm.Service;

namespace Masa.Contrib.StackSdks.Pm;

public class PmClient : IPmClient
{
    public PmClient(ICaller caller)
    {
        EnvironmentService = new EnvironmentService(caller);
        ClusterService = new ClusterService(caller);
        ProjectService = new ProjectService(caller);
        AppService = new AppService(caller);
    }

    public IProjectService ProjectService { get; init; }

    public IEnvironmentService EnvironmentService { get; init; }

    public IClusterService ClusterService { get; init; }

    public IAppService AppService { get; init; }
}

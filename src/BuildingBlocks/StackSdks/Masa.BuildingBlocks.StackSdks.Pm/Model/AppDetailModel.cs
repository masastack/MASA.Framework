// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Pm.Model;

public class AppDetailModel : ModelBase
{
    public int ProjectId { get; set; }

    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Identity { get; set; } = "";

    public string Description { get; set; } = "";

    public AppTypes Type { get; set; }

    public ServiceTypes ServiceType { get; set; }

    public string Url { get; set; } = "";

    public string SwaggerUrl { get; set; } = "";

    public List<EnvironmentClusterModel> EnvironmentClusters { get; set; } = new();
}

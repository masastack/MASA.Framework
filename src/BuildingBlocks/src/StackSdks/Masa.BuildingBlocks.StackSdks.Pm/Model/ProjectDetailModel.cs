// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Pm.Model;

public class ProjectDetailModel : BaseModel
{
    public int Id { get; set; }

    public string Identity { get; set; } = "";

    public string LabelCode { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public Guid TeamId { get; set; }

    public List<int> EnvironmentClusterIds { get; set; } = new List<int>();
}

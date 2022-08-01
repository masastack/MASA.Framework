// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Pm.Model;

public class EnvironmentDetailModel : BaseModel
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string Description { get; set; } = "";

    public string Color { get; set; } = "";

    public List<int> ClusterIds { get; set; } = default!;
}

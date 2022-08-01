// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Pm.Model;

public class ProjectAppsModel
{
    public int Id { get; set; }

    public string Identity { get; set; }

    public string Name { get; set; }

    public string LabelCode { get; set; }

    public Guid TeamId { get; set; }

    public List<AppModel> Apps { get; set; } = new();

    public ProjectAppsModel(int id, string identity, string name, string labelCode, Guid teamId)
    {
        Id = id;
        Identity = identity;
        Name = name;
        LabelCode = labelCode;
        TeamId = teamId;
    }
}

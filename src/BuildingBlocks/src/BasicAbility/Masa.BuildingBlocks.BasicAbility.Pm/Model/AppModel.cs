// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Pm.Model;

public class AppModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Identity { get; set; }

    public int ProjectId { get; set; }

    public AppTypes Type { get; set; }

    public AppModel()
    {
    }

    public AppModel(int id, string name, string identity, int projectId)
    {
        Id = id;
        Name = name;
        Identity = identity;
        ProjectId = projectId;
    }
}

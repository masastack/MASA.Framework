// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Auth.Contracts.Model;

public class ProjectModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Identity { get; set; } = string.Empty;

    public List<AppModel> Apps { get; set; } = new();
}

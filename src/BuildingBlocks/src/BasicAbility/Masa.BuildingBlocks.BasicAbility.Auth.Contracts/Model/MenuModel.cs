// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Auth.Contracts.Model;

public class MenuModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }

    public string Icon { get; set; }

    public string Url { get; set; }

    public List<MenuModel> Children { get; set; } = new();
}

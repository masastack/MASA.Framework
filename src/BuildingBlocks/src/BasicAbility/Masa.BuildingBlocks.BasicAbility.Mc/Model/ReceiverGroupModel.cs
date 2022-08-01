// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Mc.Model;

public class ReceiverGroupModel : AuditEntityModel<Guid, Guid>
{
    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<ReceiverGroupItemModel> Items { get; set; } = new();

    public string ModifierName { get; set; } = string.Empty;
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class GetSystemDataModel
{
    public string SystemId { get; set; }

    public List<Guid> UserIds { get; set; } = new();
}

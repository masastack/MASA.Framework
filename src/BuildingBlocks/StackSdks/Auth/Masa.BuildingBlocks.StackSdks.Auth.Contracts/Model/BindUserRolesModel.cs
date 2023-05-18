// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class BindUserRolesModel
{
    public Guid Id { get; set; }

    public IEnumerable<string> RoleCodes { get; set; } = new List<string>();
}

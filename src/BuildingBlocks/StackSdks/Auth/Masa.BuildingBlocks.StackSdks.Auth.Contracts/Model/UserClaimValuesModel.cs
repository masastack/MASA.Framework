// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class UserClaimValuesModel
{
    public Guid UserId { get; set; }

    public Dictionary<string, string> ClaimValues { get; set; } = new();
}

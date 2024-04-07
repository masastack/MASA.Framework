// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class AddThirdPartyUserModel
{
    public string ThridPartyIdentity { get; set; } = "";

    public object ExtendedData { get; set; }

    public string Scheme { get; set; }

    public AddUserModel User { get; set; }

    public Dictionary<string, string> ClaimData { get; set; } = new();
}

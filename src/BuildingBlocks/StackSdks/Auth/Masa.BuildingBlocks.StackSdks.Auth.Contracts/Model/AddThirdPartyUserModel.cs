// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class AddThirdPartyUserModel
{
    public string? Account { get; set; }

    public string? Name { get; set; }

    public string DisplayName { get; set; }

    public string? IdCard { get; set; }

    public string? CompanyName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public GenderTypes Gender { get; set; }

    public string ThridPartyIdentity { get; set; }

    public object? ExtendedData { get; set; }

    public ThirdPartyIdpTypes ThirdPartyIdpType { get; set; }
}

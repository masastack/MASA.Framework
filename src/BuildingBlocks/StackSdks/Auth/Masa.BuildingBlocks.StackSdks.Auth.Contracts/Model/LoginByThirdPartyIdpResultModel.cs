// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class LoginByThirdPartyIdpResultModel
{
    public TokenModel? Token { get; set; }

    public ThirdPartyIdentityModel? ThirdPartyIdentity { get; set; }

    public ThirdPartyIdpLoginResultTypes ThirdPartyIdpLoginResultType { get; set; }
}

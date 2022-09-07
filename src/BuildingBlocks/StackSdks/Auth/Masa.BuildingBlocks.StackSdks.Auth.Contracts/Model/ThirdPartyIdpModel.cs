// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class ThirdPartyIdpModel
{
    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string Url { get; set; }

    public AuthenticationTypes VerifyType { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Icon { get; set; }

    public ThirdPartyIdpTypes ThirdPartyIdpType { get; set; }
}

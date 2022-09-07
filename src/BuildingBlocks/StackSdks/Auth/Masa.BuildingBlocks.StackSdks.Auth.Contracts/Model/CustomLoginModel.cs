// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class CustomLoginModel
{
    public string Name { get; set; }

    public string Title { get; set; }

    public string ClientId { get; set; }

    public List<ThirdPartyIdpModel> ThirdPartyIdps { get; set; }

    public List<RegisterFieldModel> RegisterFields { get; set; }
}

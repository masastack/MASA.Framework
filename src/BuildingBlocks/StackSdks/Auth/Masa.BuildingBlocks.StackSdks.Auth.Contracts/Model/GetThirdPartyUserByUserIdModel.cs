// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class GetThirdPartyUserByUserIdModel : IEnvironmentModel
{
    public string Scheme { get; set; }

    public Guid UserId { get; set; }

    public string Environment { get; set; }
}

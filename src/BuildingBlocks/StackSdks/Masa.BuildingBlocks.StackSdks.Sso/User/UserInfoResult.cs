// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Sso.User;

public class UserInfoResult : ProtocolResult
{
    public IEnumerable<Claim> Claims { get; private set; } = default!;
}

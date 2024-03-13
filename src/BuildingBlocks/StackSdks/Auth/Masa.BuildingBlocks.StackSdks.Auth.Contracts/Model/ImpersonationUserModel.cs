// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class ImpersonationUserModel
{
    public Guid ImpersonatorUserId { get; set; }

    public Guid TargetUserId { get; set; }

    public bool IsBackToImpersonator { get; set; }
}

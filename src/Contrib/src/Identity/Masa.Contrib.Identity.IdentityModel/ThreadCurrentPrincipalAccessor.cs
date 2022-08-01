﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Identity.IdentityModel;

public class ThreadCurrentPrincipalAccessor : ICurrentPrincipalAccessor
{
    public virtual ClaimsPrincipal? GetCurrentPrincipal() => Thread.CurrentPrincipal as ClaimsPrincipal;
}

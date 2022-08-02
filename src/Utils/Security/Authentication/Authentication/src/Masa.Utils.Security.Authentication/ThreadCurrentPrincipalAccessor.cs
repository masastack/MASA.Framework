// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Authentication;

internal class ThreadCurrentPrincipalAccessor : ICurrentPrincipalAccessor, ISingletonDependency
{
    public ClaimsPrincipal? Principal => Thread.CurrentPrincipal as ClaimsPrincipal;
}

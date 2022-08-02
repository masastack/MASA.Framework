// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Authentication;

public interface ICurrentPrincipalAccessor
{
    ClaimsPrincipal? Principal { get; }
}

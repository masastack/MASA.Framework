// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Enums;

public enum TokenExpiration
{
    [Description("Sliding token expiration")]
    Sliding,
    [Description("Absolute token expiration")]
    Absolute
}


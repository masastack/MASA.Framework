// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;

public class ApiResourceSecret : Secret
{
    public int ApiResourceId { get; private set; }

    public ApiResource ApiResource { get; private set; } = null!;
}


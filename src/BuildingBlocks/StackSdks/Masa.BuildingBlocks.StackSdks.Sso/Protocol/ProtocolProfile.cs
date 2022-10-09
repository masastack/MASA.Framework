// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Sso.Protocol;

public abstract class ProtocolProfile
{
    public string? Address { get; set; }

    public string ClientId { get; set; } = default!;

    public string? ClientSecret { get; set; }

    public List<KeyValuePair<string, string>> Parameters { get; set; } = new();
}

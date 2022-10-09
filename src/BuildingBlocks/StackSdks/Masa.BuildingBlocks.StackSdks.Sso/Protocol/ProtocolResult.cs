// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Sso.Protocol;

public class ProtocolResult
{
    public string? Raw { get; protected set; }

    public JsonElement Json { get; protected set; }

    public Exception? Exception { get; protected set; }

    public bool IsError => string.IsNullOrEmpty(Error);

    public string? Error { get; init; }
}

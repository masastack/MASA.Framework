// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;

public class TraceExceptionResponseDto
{
    [JsonPropertyName("exception.type")]
    public virtual string Type { get; set; }

    [JsonPropertyName("exception.message")]
    public virtual string Message { get; set; }

    [JsonPropertyName("exception.stacktrace")]
    public virtual string StackTrace { get; set; }

    [JsonPropertyName("exception.escaped")]
    public virtual bool Escaped { get; set; }
}

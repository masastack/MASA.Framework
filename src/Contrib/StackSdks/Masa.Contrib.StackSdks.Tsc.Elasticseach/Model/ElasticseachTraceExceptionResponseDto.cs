// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Log.Elasticseach.Model;

internal class ElasticseachTraceExceptionResponseDto : TraceExceptionResponseDto
{
    [JsonPropertyName("exception.type")]
    public override string Type { get; set; }

    [JsonPropertyName("exception.message")]
    public override string Message { get; set; }

    [JsonPropertyName("exception.stacktrace")]
    public override string StackTrace { get; set; }

    [JsonPropertyName("exception.escaped")]
    public override bool Escaped { get; set; }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticseach.Model;

internal class ElasticseachTraceResponseDto : TraceResponseDto
{
    [JsonPropertyName("@timestamp")]
    public override DateTime Timestamp { get; set; }

    [JsonPropertyName("EndTimestamp")]
    public override DateTime EndTimestamp { get; set; }

    public override bool IsDatabase(out TraceDatabaseResponseDto result)
    {
        var success = this.IsDatabase(out ElasticseachTraceDatabaseResponseDto elasticResult);
        result = success ? elasticResult : default!;
        return success;
    }

    public override bool IsException(out TraceExceptionResponseDto result)
    {
        var success = this.IsException(out ElasticseachTraceExceptionResponseDto elasticResult);
        result = success ? elasticResult : default!;
        return success;
    }

    public override bool IsHttp(out TraceHttpResponseDto result)
    {
        var success = this.IsHttp(out ElasticseachTraceHttpResponseDto elasticResult);
        result = success ? elasticResult : default!;
        return success;
    }
}

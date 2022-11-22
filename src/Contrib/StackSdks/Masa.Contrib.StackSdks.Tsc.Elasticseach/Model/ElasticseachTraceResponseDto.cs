// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticseach.Model;

internal class ElasticseachTraceResponseDto : TraceResponseDto
{
    [JsonPropertyName("@timestamp")]
    public override DateTime Timestamp { get; set; }

    [JsonPropertyName("EndTimestamp")]
    public override DateTime EndTimestamp { get; set; }

    public override bool TryParseDatabase(out TraceDatabaseResponseDto result)
    {
        var success = this.TryParseDatabase(out ElasticseachTraceDatabaseResponseDto elasticResult);
        result = success ? elasticResult : default!;
        return success;
    }

    public override bool TryParseException(out TraceExceptionResponseDto result)
    {
        var success = this.TryParseException(out ElasticseachTraceExceptionResponseDto elasticResult);
        result = success ? elasticResult : default!;
        return success;
    }

    public override bool TryParseHttp(out TraceHttpResponseDto result)
    {
        var success = this.TryParseHttp(out ElasticseachTraceHttpResponseDto elasticResult);
        result = success ? elasticResult : default!;
        return success;
    }
}

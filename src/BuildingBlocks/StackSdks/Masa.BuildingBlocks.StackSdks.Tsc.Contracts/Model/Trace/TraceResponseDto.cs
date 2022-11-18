// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;

public class TraceResponseDto
{
    public virtual DateTime Timestamp { get; set; }

    public virtual DateTime EndTimestamp { get; set; }

    public virtual string TraceId { get; set; }

    public virtual string SpanId { get; set; }

    public virtual string ParentSpanId { get; set; }

    public virtual string Kind { get; set; }

    public virtual int TraceStatus { get; set; }

    public virtual string Name { get; set; }

    public virtual Dictionary<string, object> Attributes { get; set; }

    public virtual Dictionary<string, object> Resource { get; set; }

    public virtual long Duration => (long)Math.Floor((EndTimestamp - Timestamp).TotalMilliseconds);

    public virtual bool TryParseHttp(out TraceHttpResponseDto result)
    {
        result = default!;
        return false;
    }    

    public virtual bool TryParseDatabase(out TraceDatabaseResponseDto result)
    {
        result = default!;
        return false;
    }

    public virtual bool TryParseException(out TraceExceptionResponseDto result)
    {
        result = default!;
        return false;
    }

    public virtual string GetDispalyName()
    {
        if (TryParseHttp(out var traceHttpDto))
        {
            if (Kind == TraceDtoKind.SPAN_KIND_SERVER)
                return traceHttpDto.Target;
            return traceHttpDto.Url;
        }
        else if (TryParseDatabase(out var databaseDto))
        {
            return databaseDto.Name;
        }
        else if (TryParseException(out TraceExceptionResponseDto exceptionDto))
        {
            return exceptionDto.Type ?? exceptionDto.Message;
        }
        else
            return Name;
    }
}

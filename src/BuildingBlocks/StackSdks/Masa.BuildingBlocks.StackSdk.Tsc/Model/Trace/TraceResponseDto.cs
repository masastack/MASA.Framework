// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Model.Trace;

public abstract class TraceResponseDto
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
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;

public class TraceExceptionResponseDto
{
    public virtual string Type { get; set; }

    public virtual string Message { get; set; }

    public virtual string StackTrace { get; set; }

    public virtual bool Escaped { get; set; }
}

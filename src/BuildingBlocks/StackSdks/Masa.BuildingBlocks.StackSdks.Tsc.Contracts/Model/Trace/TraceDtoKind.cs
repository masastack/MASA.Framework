// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model.Trace;

public sealed class TraceDtoKind
{
    private TraceDtoKind() { }

    public const string SPAN_KIND_SERVER = nameof(SPAN_KIND_SERVER);

    public const string SPAN_KIND_CLIENT = nameof(SPAN_KIND_CLIENT);
}

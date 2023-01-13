// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient.Tests;

public class DocDaprCaller : DaprCallerBase
{
    protected override string AppId { get; set; } = "doc";

    public ICaller GetCaller() => Caller;
}

﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.IntegratedTest.Callers;

public class DaprCaller : DaprCallerBase
{
    public DaprCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        AppId = "DaprCaller";
    }

    protected override string AppId { get; set; }

    public bool CallerProviderIsNotNull() => CallerProvider != null;
}

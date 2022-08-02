// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Tests.Infrastructure.Callers;

public class DaprCaller : DaprCallerBase
{
    protected override string AppId { get; set; } = "DaprCaller";

    public DaprCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public bool CallerProviderIsNotNull() => CallerProvider != null;
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Core.Tests.Callers;

public class GithubCaller : HttpClientCallerBase
{
    public GithubCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override string BaseAddress { get; set; } = default!;
}

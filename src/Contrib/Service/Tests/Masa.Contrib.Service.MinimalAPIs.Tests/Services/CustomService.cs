// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests.Services;

public class CustomService : CustomServiceBase
{
    private readonly int _times;

    public CustomService(IServiceCollection services) : base(services)
    {
        _times++;
    }

#pragma warning disable CA1822
    public void Test1()
    {
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public int GetTest2() => _times;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public Task PostTest3(int a, int b) => Task.FromResult(a + b);

    public Task<int> PutTest4(int a, int b) => Task.FromResult(a + b);
#pragma warning restore CA1822
}

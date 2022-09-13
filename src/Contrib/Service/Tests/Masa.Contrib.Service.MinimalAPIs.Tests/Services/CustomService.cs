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

    public void Test1()
    {
    }

    public int GetTest2() => _times;

    public Task PostTest3(int a, int b) => Task.FromResult(a + b);

    public Task<int> PutTest4(int a, int b) => Task.FromResult(a + b);
}

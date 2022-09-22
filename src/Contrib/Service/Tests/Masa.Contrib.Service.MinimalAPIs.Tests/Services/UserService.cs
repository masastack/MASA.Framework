// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests.Services;

public class UserService : CustomServiceBase
{
    public UserService()
    {

    }

    public UserService(bool? disableTrimStartMethodPrefix)
    {
        RouteOptions.DisableTrimMethodPrefix = disableTrimStartMethodPrefix;
    }

    public UserService(string[] defaultHttpMethods)
    {
        RouteOptions.MapHttpMethodsForUnmatched = defaultHttpMethods;
    }

#pragma warning disable CA1822
    public void Test(int id)
    {
    }

    public void Test2(int? id)
    {
    }

    public void Test3([FromQuery] int? id)
    {
    }

    public void Test4([FromBody] int? id)
    {
    }

    public void Test5([FromHeader] int? id)
    {
    }

    public void Test6([FromForm] int? id)
    {
    }

    public void Test7([FromRoute] int? id)
    {
    }
#pragma warning restore CA1822
}

public class TestDto
{
    public string? Id { get; set; }
}

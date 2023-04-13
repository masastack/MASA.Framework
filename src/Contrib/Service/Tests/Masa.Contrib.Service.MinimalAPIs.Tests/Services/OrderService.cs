﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests.Services;

public class OrderService : ServiceBase
{
    public OrderService(bool? enableProperty)
    {
        RouteOptions.EnableProperty = enableProperty;
    }

    public string ConnectionString => GetConnectionString;

    public static string GetConnectionString => "connection string";

    private int Age { get; set; }

    private static string GetName() => "name";

    public static void SetName()
    {

    }

    public override string ToString()
    {
        return base.ToString();
    }

    [IgnoreRoute]
    public List<MethodInfo> TestGetMethodsByAutoMapRoute(ServiceGlobalRouteOptions globalOptions)
    {
        return base.GetMethodsByAutoMapRoute(typeof(OrderService), globalOptions);
    }
}

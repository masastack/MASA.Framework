// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests;

[TestClass]
public class RoutePatternTest
{
    [TestMethod]
    public void TestRoutePattern()
    {
        var routePattern = new RoutePatternAttribute();
        Assert.AreEqual(null, routePattern.Pattern);
        Assert.AreEqual(false, routePattern.StartWithBaseUri);
        Assert.AreEqual(null, routePattern.HttpMethod);

        routePattern = new RoutePatternAttribute("/api/v1/catalog");
        Assert.AreEqual("/api/v1/catalog", routePattern.Pattern);
        Assert.AreEqual(false, routePattern.StartWithBaseUri);
        Assert.AreEqual(null, routePattern.HttpMethod);

        routePattern = new RoutePatternAttribute("/api/v1/catalog", true)
        {
            HttpMethod = "Get"
        };
        Assert.AreEqual("/api/v1/catalog", routePattern.Pattern);
        Assert.AreEqual(true, routePattern.StartWithBaseUri);
        Assert.AreEqual("Get", routePattern.HttpMethod);
    }
}

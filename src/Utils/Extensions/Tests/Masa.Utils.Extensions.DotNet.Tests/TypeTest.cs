// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DotNet.Tests;

[TestClass]
public class TypeTest
{
    [TestMethod]
    public void TestIsImplementerOfGeneric()
    {
        var res = typeof(Demo).IsImplementerOfGeneric(typeof(IEnumerable<>));
        var res2 = typeof(Demo).IsImplementerOfGeneric(typeof(IList<>));
        Assert.IsTrue(res);
        Assert.IsTrue(res2);
    }

    public class Demo : List<int>
    {

    }
}

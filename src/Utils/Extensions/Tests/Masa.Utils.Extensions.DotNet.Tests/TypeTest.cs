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
        Assert.IsTrue(res);

        res = typeof(Demo).IsImplementerOfGeneric(typeof(IList<>));
        Assert.IsTrue(res);

        res = typeof(Repository<>).IsImplementerOfGeneric(typeof(IRepository<>));
        Assert.IsTrue(res);

        res = typeof(Repository<,>).IsImplementerOfGeneric(typeof(IRepository<>));
        Assert.IsTrue(res);

        res = typeof(IRepository<,>).IsImplementerOfGeneric(typeof(IRepository<>));
        Assert.IsTrue(res);

        res = typeof(UserRepository).IsImplementerOfGeneric(typeof(Repository<>));
        Assert.IsTrue(res);

        res = typeof(Repository<,>).IsImplementerOfGeneric(typeof(Repository<>));
        Assert.IsTrue(res);

        res = typeof(String).IsImplementerOfGeneric(typeof(IEquatable<>));
        Assert.IsTrue(res);
    }

    public class Demo : List<int>
    {

    }
}

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

    [TestMethod]
    public void TestGetDefaultValue()
    {
        Assert.AreEqual(0, typeof(int).GetDefaultValue());
        Assert.AreEqual(null, typeof(string).GetDefaultValue());
        Assert.AreEqual(0, (float)typeof(float).GetDefaultValue()!);
        Assert.AreEqual(0, (decimal)typeof(decimal).GetDefaultValue()!);
        Assert.AreEqual(0, (double)typeof(double).GetDefaultValue()!);
        Assert.AreEqual(Guid.Empty, (Guid)typeof(Guid).GetDefaultValue()!);
        Assert.AreEqual(null, typeof(List<string>).GetDefaultValue());
        Assert.AreEqual(null, typeof(string[]).GetDefaultValue());
        Assert.IsNull(typeof(Dictionary<string, string>).GetDefaultValue());
    }

    public class Demo : List<int>
    {

    }
}

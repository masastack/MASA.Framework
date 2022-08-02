// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration.Tests;

[TestClass]
public class TestProperties
{
    [TestMethod]
    public void TestEquals()
    {
        var id = Guid.NewGuid().ToString();
        var x = new Properties(new Dictionary<string, string>()
        {
            {"id", id},
        });
        var y = new Properties(new Dictionary<string, string>()
        {
            {"id", id},
        });
        var z = new Properties(new Dictionary<string, string>()
        {
            {"id2", Guid.NewGuid().ToString()},
            {"id", id}
        });
        Assert.IsTrue(x.Equals(y));
        Assert.IsTrue(!x.Equals(null));
        Assert.IsTrue(!x!.Equals(z));
        Assert.IsTrue(!z.Equals(x));

        var w = new Properties(new Dictionary<string, string>()
        {
            {"id", Guid.NewGuid().ToString()},
        });

        Assert.IsFalse(x!.Equals(w));
    }

    [TestMethod]
    public void TestGetHashCode()
    {
        var id = Guid.NewGuid().ToString();
        Properties x = new(new Dictionary<string, string>() { { "Id", id } });
        Properties y = new(new Dictionary<string, string>() { { "Id", id } });

        Assert.AreEqual(x.GetHashCode(), y.GetHashCode());
    }

    [TestMethod]
    public void TestOperator()
    {
        var id = Guid.NewGuid().ToString();
        Properties x = new(new Dictionary<string, string>() { { "Id", id } });
        Properties y = new(new Dictionary<string, string>() { { "Id", id } });
        Properties z = new(new Dictionary<string, string>() { { "Id", Guid.NewGuid().ToString() } });

        Assert.IsTrue(x == y);
        Assert.IsTrue(x != z);

        Properties? m = null;
        Assert.IsTrue(m == null);
        Assert.IsTrue(null == m);
        Assert.IsFalse(null != m);
        Assert.IsFalse(m != null);
    }
}

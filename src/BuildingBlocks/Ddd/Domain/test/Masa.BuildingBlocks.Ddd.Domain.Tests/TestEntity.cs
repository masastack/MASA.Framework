// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Tests;

[TestClass]
public class TestEntity
{
    [TestMethod]
    public void TestToString()
    {
        MasaEntity entity = new(Guid.Empty);
        Assert.AreEqual("MasaEntity:Id=00000000-0000-0000-0000-000000000000", entity.ToString());
    }

    [TestMethod]
    public void TestEquals()
    {
        var id = Guid.NewGuid();
        MasaEntity x = new(id);
        MasaEntity y = new(id);

        Assert.IsTrue(x.Equals(y));
        Assert.IsTrue(x.Equals((object)y));
    }

    [TestMethod]
    public void TestGetHashCode()
    {
        var id = Guid.NewGuid();
        MasaEntity x = new(id);
        MasaEntity y = new(id);

        Assert.AreEqual(x.GetHashCode(), y.GetHashCode());
    }

    [TestMethod]
    public void TestOperator()
    {
        var id = Guid.NewGuid();
        MasaEntity x = new(id);
        MasaEntity y = new(id);
        MasaEntity z = new(Guid.NewGuid());

        Assert.IsTrue(x == y);
        Assert.IsTrue(x != z);

        MasaEntity? m = null;
        Assert.IsTrue(m == null);
        Assert.IsTrue(null == m);
        Assert.IsFalse(null != m);
        Assert.IsFalse(m != null);
        Assert.IsTrue(x != null);
        Assert.IsTrue(null != x);
    }

    public class MasaEntity : Entity<Guid>
    {
        public MasaEntity(Guid id)
        {
            Id = id;
        }
    }
}

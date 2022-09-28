// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Tests;

[TestClass]
public class ValueObjectTest
{
    [TestMethod]
    public void TestEquals()
    {
        var id = Guid.NewGuid();
        MasaValueObject x = new() { Id = id };
        MasaValueObject y = new() { Id = id };

        Assert.IsTrue(x.Equals(y));
    }

    [TestMethod]
    public void TestGetHashCode()
    {
        var id = Guid.NewGuid();
        MasaValueObject x = new() { Id = id };
        MasaValueObject y = new() { Id = id };

        Assert.AreEqual(x.GetHashCode(), y.GetHashCode());
    }

    [TestMethod]
    public void TestOperator()
    {
        var id = Guid.NewGuid();
        MasaValueObject x = new() { Id = id };
        MasaValueObject y = new() { Id = id };
        MasaValueObject z = new() { Id = Guid.NewGuid() };

        Assert.IsTrue(x == y);
        Assert.IsTrue(x != z);
    }

    [TestMethod]
    public void TestDistinct()
    {
        var id = Guid.NewGuid();
        MasaValueObject x = new() { Id = id };
        MasaValueObject y = new() { Id = id };
        MasaValueObject z = new() { Id = Guid.NewGuid() };

        var list = new List<MasaValueObject>()
        {
            x,
            y,
            z
        };
        Assert.IsTrue(list.Distinct().Count() == 2);
        Assert.IsTrue(list.Contains(x));
        Assert.IsTrue(list.Contains(z));
    }

    [TestMethod]
    public void TestContainer()
    {
        var id = Guid.NewGuid();
        MasaValueObject x = new() { Id = id };
        MasaValueObject y = new() { Id = id };
        MasaValueObject z = new() { Id = Guid.NewGuid() };

        var list = new List<MasaValueObject>()
        {
            x,
            y
        };
        Assert.IsTrue(list.Distinct().Count() == 1);
        Assert.IsTrue(list.Contains(x));
        Assert.IsTrue(list.Contains(y));
        Assert.IsFalse(list.Contains(z));
    }

    public class MasaValueObject : ValueObject
    {
        public Guid Id { get; set; }

        protected override IEnumerable<object> GetEqualityValues()
        {
            yield return Id;
        }
    }
}

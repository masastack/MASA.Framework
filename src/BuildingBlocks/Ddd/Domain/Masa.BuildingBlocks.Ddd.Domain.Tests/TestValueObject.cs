using Masa.BuildingBlocks.Ddd.Domain.Values;

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Tests;

[TestClass]
public class TestValueObject
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

    public class MasaValueObject : ValueObject
    {
        public Guid Id { get; set; }

        protected override IEnumerable<object> GetEqualityValues()
        {
            yield return Id;
        }
    }
}

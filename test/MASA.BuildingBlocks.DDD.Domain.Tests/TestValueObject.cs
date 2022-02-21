using MASA.BuildingBlocks.DDD.Domain.Values;

namespace MASA.BuildingBlocks.DDD.Domain.Tests;
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
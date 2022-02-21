namespace MASA.BuildingBlocks.DDD.Domain.Tests;

[TestClass]
public class TestEntity
{
    [TestMethod]
    public void TestToString()
    {
        MASAEntity entity = new() { Id = Guid.Empty };
        Assert.AreEqual("MasaEntity:Id=00000000-0000-0000-0000-000000000000", entity.ToString());
    }

    [TestMethod]
    public void TestEquals()
    {
        var id = Guid.NewGuid();
        MASAEntity x = new() { Id = id };
        MASAEntity y = new() { Id = id };

        Assert.IsTrue(x.Equals(y));
        Assert.IsTrue(x.Equals((object)y));
    }

    [TestMethod]
    public void TestGetHashCode()
    {
        var id = Guid.NewGuid();
        MASAEntity x = new() { Id = id };
        MASAEntity y = new() { Id = id };

        Assert.AreEqual(x.GetHashCode(), y.GetHashCode());
    }

    [TestMethod]
    public void TestOperator()
    {
        var id = Guid.NewGuid();
        MASAEntity x = new() { Id = id };
        MASAEntity y = new() { Id = id };
        MASAEntity z = new() { Id = Guid.NewGuid() };

        Assert.IsTrue(x == y);
        Assert.IsTrue(x != z);

        MASAEntity? m = null;
        Assert.IsTrue(m == null);
        Assert.IsTrue(null == m);
        Assert.IsFalse(null != m);
        Assert.IsFalse(m != null);
        Assert.IsTrue(x != null);
        Assert.IsTrue(null != x);
    }

    public class MASAEntity : Entity<Guid>
    {
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.Redis.Tests;

[TestClass]
public class ConvertToValueTest
{
    [TestMethod]
    public void TestConvertByGuid()
    {
        Guid id = Guid.NewGuid();
        RedisValue obj = RedisHelper.ConvertFromValue(id);
        Assert.IsTrue(RedisHelper.ConvertToValue<Guid>(obj) == id);
    }

    [TestMethod]
    public void TestConvertByByte()
    {
        byte id = 1;
        RedisValue obj = RedisHelper.ConvertFromValue(id);
        Assert.IsTrue(RedisHelper.ConvertToValue<byte>(obj) == id);
    }

    [TestMethod]
    public void TestConvertBySByte()
    {
        sbyte id = 1;
        RedisValue obj = RedisHelper.ConvertFromValue(id);
        Assert.IsTrue(RedisHelper.ConvertToValue<sbyte>(obj) == id);
    }

    [TestMethod]
    public void TestConvertByUShort()
    {
        ushort id = 1;
        RedisValue obj = RedisHelper.ConvertFromValue(id);
        Assert.IsTrue(RedisHelper.ConvertToValue<ushort>(obj) == id);
    }

    [TestMethod]
    public void TestConvertByUInt()
    {
        uint id = 1;
        RedisValue obj = RedisHelper.ConvertFromValue(id);
        Assert.IsTrue(RedisHelper.ConvertToValue<uint>(obj) == id);
    }

    [TestMethod]
    public void TestConvertByULong()
    {
        ulong id = 1;
        RedisValue obj = RedisHelper.ConvertFromValue(id);
        Assert.IsTrue(RedisHelper.ConvertToValue<ulong>(obj) == id);
    }

    [TestMethod]
    public void TestConvertByShort()
    {
        short id = 1;
        RedisValue obj = RedisHelper.ConvertFromValue(id);
        Assert.IsTrue(RedisHelper.ConvertToValue<short>(obj) == id);
    }

    [TestMethod]
    public void TestConvertByInt()
    {
        int id = 1;
        RedisValue obj = RedisHelper.ConvertFromValue(id);
        Assert.IsTrue(RedisHelper.ConvertToValue<int>(obj) == id);
    }

    [TestMethod]
    public void TestConvertByLong()
    {
        long id = 1;
        RedisValue obj = RedisHelper.ConvertFromValue(id);
        Assert.IsTrue(RedisHelper.ConvertToValue<long>(obj) == id);
    }

    [TestMethod]
    public void TestConvertByDouble()
    {
        double score = 1.1d;
        RedisValue obj = RedisHelper.ConvertFromValue(score);
        Assert.IsTrue(RedisHelper.ConvertToValue<double>(obj) == score);
    }

    [TestMethod]
    public void TestConvertByFloat()
    {
        float score = 1.1f;
        RedisValue obj = RedisHelper.ConvertFromValue(score);
        Assert.IsTrue(RedisHelper.ConvertToValue<float>(obj) == score);
    }

    [TestMethod]
    public void TestConvertByDecimal()
    {
        decimal score = 1.1m;
        dynamic obj = RedisHelper.ConvertFromValue(score);
        Assert.IsTrue(RedisHelper.ConvertToValue<decimal>(obj) == score);
    }

    [TestMethod]
    public void TestConvertByDynamic()
    {
        dynamic user = new
        {
            Name = "Jim"
        };
        RedisValue obj = RedisHelper.ConvertFromValue(user)!;
        Assert.IsTrue(RedisHelper.ConvertToValue<dynamic>(obj)!.Name == user.Name);
    }
}

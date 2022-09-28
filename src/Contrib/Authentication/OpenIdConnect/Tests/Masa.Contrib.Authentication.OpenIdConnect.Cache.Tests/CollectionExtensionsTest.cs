// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.Cache.Tests;

[TestClass]
public class CollectionExtensionsTest
{
    [TestMethod]
    public void TestSet()
    {
        var data = new List<Identity> {
            new(1,"张三"),
            new(2, "李四"),
            new(3, "王五"),
            new(4, "赵六")
        };
        data.Set(new(2, "李蛋"), item => item.Id);
        Assert.IsTrue(data.First(item => item.Id == 2).Name == "李蛋");
    }

    [TestMethod]
    public void TestSetRange()
    {
        var data = new List<Identity> {
            new(1,"张三"),
            new(2, "李四"),
            new(3, "王五"),
            new(4, "赵六")
        };
        data.SetRange(new List<Identity>
        {
            new(1,"张队"),
            new(2, "李蛋"),
        }, item => item.Id);
        Assert.IsTrue(data.First(item => item.Id == 1).Name == "张队");
        Assert.IsTrue(data.First(item => item.Id == 2).Name == "李蛋");
    }

    [TestMethod]
    public void TestRemove()
    {
        var data = new List<Identity> {
            new(1,"张三"),
            new(2, "李四"),
            new(3, "王五"),
            new(4, "赵六")
        };
        data.Remove(new(1, ""), item => item.Id);
        Assert.IsTrue(data.Any(item => item.Id == 1) is false);
    }

    [TestMethod]
    public void TestRemove2()
    {
        var data = new List<Identity> {
            new(1,"张三"),
            new(2, "李四"),
            new(3, "王五"),
            new(4, "赵六")
        };
        data.Remove(item => item.Id == 1);
        Assert.IsTrue(data.Any(item => item.Id == 1) is false);
    }

    [TestMethod]
    public void TestRemoveRange()
    {
        var data = new List<Identity> {
            new(1,"张三"),
            new(2, "李四"),
            new(3, "王五"),
            new(4, "赵六")
        };
        data.RemoveRange(new List<Identity>
        {
            new(1,""),
            new(2, ""),
        }, item => item.Id);
        Assert.IsTrue(data.Any(item => item.Id == 1) is false);
        Assert.IsTrue(data.Any(item => item.Id == 2) is false);
    }
}

class Identity
{
    public int Id { get; set; }

    public string Name { get; set; }

    public Identity(int id, string name)
    {
        Id = id;
        Name = name;
    }
}


// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

#pragma warning disable CS0618
namespace Masa.Utils.Caller.Tests;

[TestClass]
public class TypeConvertTest
{
    [TestMethod]
    public void TestConvertToKeyValuePairs()
    {
        var defaultTypeConvertProvider = new DefaultTypeConvertProvider();
        var result = defaultTypeConvertProvider.ConvertToKeyValuePairs(new
        {
            id = 1,
            name = "masa"
        }).ToList();
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.Any(x => x.Key == "id" && x.Value == "1"));
        Assert.IsTrue(result.Any(x => x.Key == "name" && x.Value == "masa"));

        result = defaultTypeConvertProvider.ConvertToKeyValuePairs(new
        {
            id = 2,
            text = "masa"
        }).ToList();
        Assert.IsTrue(result.Any(x => x.Key == "id" && x.Value == "2"));
        Assert.IsTrue(result.Any(x => x.Key == "text" && x.Value == "masa"));
    }

    [TestMethod]
    public void TestConvertToDictionaryByDynamic()
    {
        var provider = new DefaultTypeConvertProvider();
        var dictionary = new Dictionary<string, string>
        {
            { "account", "jim" },
            { "age", "18" }
        };
        var request = new
        {
            account = "jim",
            age = 18
        };
        var result = provider.ConvertToDictionary(request);
        Assert.IsTrue(System.Text.Json.JsonSerializer.Serialize(result) == System.Text.Json.JsonSerializer.Serialize(dictionary));
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject()
    {
        var provider = new DefaultTypeConvertProvider();
        var query = new UserListQury("Jim");
        var dictionary = new Dictionary<string, string>
        {
            { "name", query.Name }
        };
        var result = provider.ConvertToDictionary(query);
        Assert.IsTrue(System.Text.Json.JsonSerializer.Serialize(result) == System.Text.Json.JsonSerializer.Serialize(dictionary));
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject2()
    {
        var provider = new DefaultTypeConvertProvider();
        var query = new UserDetailQury("Jim", "Music", "Game");
        var result = provider.ConvertToDictionary(query);
        Assert.IsTrue(result.Count == 2);
        Assert.IsTrue(result["name"] == query.Name);
        Assert.IsTrue(result["Tags"] == System.Text.Json.JsonSerializer.Serialize(new List<string>()
        {
            "Music",
            "Game"
        }));
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject3()
    {
        var provider = new DefaultTypeConvertProvider();

        List<string> tags = null!;
        var query = new UserDetailQury("Jim", tags);
        var result = provider.ConvertToDictionary(query);

        Assert.IsTrue(result.Count == 1);
        Assert.IsTrue(result["name"] == query.Name);
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject4()
    {
        var provider = new DefaultTypeConvertProvider();
        var query = new UserDetailQury(null!, "Music", "Game");
        var result = provider.ConvertToDictionary(query);
        Assert.IsTrue(result.Count == 1);
        Assert.IsTrue(result["Tags"] == System.Text.Json.JsonSerializer.Serialize(new List<string>()
        {
            "Music",
            "Game"
        }));
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject5()
    {
        var provider = new DefaultTypeConvertProvider();
        var dic = new Dictionary<string, string>()
        {
            { "Account", "Jim" }
        };
        var result = provider.ConvertToDictionary(dic);
        Assert.IsTrue(result.Count == 1);
        Assert.IsTrue(result["Account"] == "Jim");
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject6()
    {
        var provider = new DefaultTypeConvertProvider();
        var dic = new List<KeyValuePair<string, string>>()
        {
            new("Account", "Jim")
        };
        var result = provider.ConvertToDictionary(dic);
        Assert.IsTrue(result.Count == 1);
        Assert.IsTrue(result["Account"] == "Jim");
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticseach.Tests.Extensions;

[TestClass]
public class JsonElementExtensionsTests
{
    [TestMethod]
    public void ArrayTest()
    {
        var str = "[]";
        var json = JsonSerializer.Deserialize<JsonElement>(str);
        var result = json.ToKeyValuePairs();
        Assert.IsNull(result);
    }

    [TestMethod]
    public void ObjectTest()
    {
        var str = "{\"name\":\"David\"}";
        var json = JsonSerializer.Deserialize<JsonElement>(str);
        var result = json.ToKeyValuePairs();
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
    }

    [TestMethod]
    public void MixObjectTest()
    {
        var str = "{\"name\":\"David\",\"age\":20,\"level\":[1,2,5,6],\"parent\":{\"name\":\"David Parent\",\"age\":52,\"level\":[999]}}";
        var json = JsonSerializer.Deserialize<JsonElement>(str);
        var result = json.ToKeyValuePairs();
        Assert.IsNotNull(result);
        var find = result.FirstOrDefault(m => m.Key == "age");
        Assert.IsNotNull(find);
        Assert.AreEqual(20,find.Value);

        find = result.FirstOrDefault(m => m.Key == "level");
        Assert.IsNotNull(find);
        IEnumerable<int> nums =((IEnumerable<object>)find.Value).Select(n=>(int)n);
        Assert.IsTrue(nums.Any());

        var parent = result.FirstOrDefault(m => m.Key == "parent");
        Assert.IsNotNull(parent);
        IEnumerable<KeyValuePair<string, object>> parentNode = (IEnumerable<KeyValuePair<string, object>>)parent.Value;
        Assert.IsNotNull(parentNode);
        Assert.IsTrue(parentNode.Any());
    }
}

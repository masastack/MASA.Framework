// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticsearch.Tests.Extensions;

[TestClass]
public class DictionaryExtenistionsTests
{
    readonly JsonSerializerOptions options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [TestMethod]
    public void ConvertDicTest()
    {
        var str = "{\"a.name\":\"David\",\"a.age\":20,\"sex\":\"Male\"}";
        var dic = JsonSerializer.Deserialize<Dictionary<string, object>>(str, options);
        Assert.IsNotNull(dic);
        var convert = dic.GroupByKeyPrefix<object>("a.");
        Assert.IsNotNull(convert);
        Assert.IsTrue(convert.ContainsKey("name"));
        Assert.IsTrue(convert.ContainsKey("age"));
    }
}

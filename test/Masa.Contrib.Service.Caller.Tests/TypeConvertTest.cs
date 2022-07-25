// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Tests;

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
}

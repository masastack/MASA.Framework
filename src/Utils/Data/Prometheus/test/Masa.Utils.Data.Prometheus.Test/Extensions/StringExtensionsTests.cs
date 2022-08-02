// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Prometheus.Test;

[TestClass]
public class StringExtensionsTests
{
    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("name")]
    [DataRow("Name")]
    [DataRow("N")]
    [DataRow("FirstName")]
    public void CamelCaseTest(string str)
    {
        var result = str.ToCamelCase();
        if (string.IsNullOrEmpty(str))
        {
            Assert.IsNull(result);
        }
        else
        {
            var camelStr = $"{str[0].ToString().ToLower()}{(str.Length - 1 > 0 ? str[1..] : "")}";
            Assert.AreEqual(camelStr, result);
        }

    }
}

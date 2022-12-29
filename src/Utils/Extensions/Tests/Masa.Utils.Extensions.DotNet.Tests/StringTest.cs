// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DotNet.Tests;

[TestClass]
public class StringTest
{
    [DataTestMethod]
    [DataRow("abcdawDf", "abc", "dawDf")]
    [DataRow("abcdawDf", "abd", "abcdawDf")]
    public void TestTrimStart(string value, string trimParameter, string result)
    {
        Assert.AreEqual(result, value.TrimStart(trimParameter, StringComparison.OrdinalIgnoreCase));
    }

    [DataTestMethod]
    [DataRow("abcdawDf", "df", "abcdaw")]
    [DataRow("abcdawDf", "adf", "abcdawDf")]
    public void TestTrimEnd(string value, string trimParameter, string result)
    {
        Assert.AreEqual(result, value.TrimEnd(trimParameter, StringComparison.OrdinalIgnoreCase));
    }

    [DataTestMethod]
    [DataRow("UserName", "User_Name", null)]
    [DataRow("UserName", "User_Name", "en-US")]
    [DataRow("userName", "User_Name", null)]
    [DataRow("userName", "User_Name", "en-US")]
    public void TestCamelCaseToSnakeCase(string value, string expected, string? culture)
    {
        var actual = culture == null ? value.CamelCaseToSnakeCase() : value.CamelCaseToSnakeCase(new CultureInfo(culture));
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("UserName", "user_name", null)]
    [DataRow("UserName", "user_name", "en-US")]
    [DataRow("userName", "user_name", null)]
    [DataRow("userName", "user_name", "en-US")]
    public void TestCamelCaseToLowerSnakeCase(string value, string expected, string? culture)
    {
        var actual = culture == null ? value.CamelCaseToLowerSnakeCase() : value.CamelCaseToLowerSnakeCase(new CultureInfo(culture));
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("User_Name", "UserName", null)]
    [DataRow("User_Name", "UserName", "en-US")]
    [DataRow("user_name", "UserName", null)]
    [DataRow("user_name", "UserName", "en-US")]
    [DataRow("user_Name", "UserName", null)]
    [DataRow("user_Name", "UserName", "en-US")]
    [DataRow("User_name", "UserName", null)]
    [DataRow("User_name", "UserName", "en-US")]
    [DataRow("_name", "Name", null)]
    [DataRow("_name", "Name", "en-US")]
    [DataRow("User_", "User", null)]
    [DataRow("User_", "User", "en-US")]
    public void TestSnakeCaseToCamelCase(string value, string expected, string? culture)
    {
        var actual = culture == null ? value.SnakeCaseToCamelCase() : value.SnakeCaseToCamelCase(new CultureInfo(culture));
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("User_Name", "userName", null)]
    [DataRow("User_Name", "userName", "en-US")]
    [DataRow("user_name", "userName", null)]
    [DataRow("user_name", "userName", "en-US")]
    [DataRow("user_Name", "userName", null)]
    [DataRow("user_Name", "userName", "en-US")]
    [DataRow("User_name", "userName", null)]
    [DataRow("User_name", "userName", "en-US")]
    [DataRow("_name", "name", null)]
    [DataRow("_name", "name", "en-US")]
    [DataRow("User_", "user", null)]
    [DataRow("User_", "user", "en-US")]
    public void TestSnakeCaseToLowerCamelCase(string value, string expected, string? culture)
    {
        var actual = culture == null ? value.SnakeCaseToLowerCamelCase() : value.SnakeCaseToLowerCamelCase(new CultureInfo(culture));
        Assert.AreEqual(expected, actual);
    }
}

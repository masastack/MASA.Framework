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
    [DataRow("User-Name", "userName")]
    [DataRow("User_Name", "userName")]
    [DataRow("user Name", "userName")]
    public void TestToCamelCase(string value, string expected)
    {
        var actual = value.ToCamelCase();
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("User-Name", "UserName")]
    [DataRow("User_Name", "UserName")]
    [DataRow("user Name", "UserName")]
    public void TestToPascalCase(string value, string expected)
    {
        var actual = value.ToPascalCase();
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("ThisIsSampleSentence", "This is sample sentence")]
    [DataRow("MASAStack", "Masa stack")]
    public void TestToSentenceCase(string value, string expected)
    {
        var actual = value.ToSentenceCase();
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("ThisIsSampleSentence", "this-is-sample-sentence")]
    [DataRow("UserID", "user-id")]
    [DataRow("userID", "user-id")]
    [DataRow("MASAAuth", "masa-auth")]
    public void TestToKebabCase(string value, string expected)
    {
        var actual = value.ToKebabCase();
        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow("ThisIsSampleSentence", "this_is_sample_sentence")]
    [DataRow("UserID", "user_id")]
    [DataRow("MASAAuth", "masa_auth")]
    public void TestToSnakeCase(string value, string expected)
    {
        var actual = value.ToSnakeCase();
        Assert.AreEqual(expected, actual);
    }
}

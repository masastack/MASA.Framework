// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography.Tests;

[TestClass]
// ReSharper disable once InconsistentNaming
public class SHAUtilsTest
{
    [TestMethod]
    public void TestEncrypt()
    {
        var str = "Hello MASA Stack";
        var encryptResult = SHA256Utils.Encrypt(str);
        Assert.AreEqual("577da9f7698725d8ac8fc73e70b182b5ae47edaf5c2be73524861b3bf0f148dc", encryptResult);
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography.Tests;

[TestClass]
public class AesTest
{
    [TestMethod]
    public void EncryptAndDecrypt()
    {
        string str = "Hello MASA Stack";
        string key = "12345678901234567890123456789021";
        var source = AesUtils.Decrypt(AesUtils.Encrypt(str, key), key);
        Assert.IsTrue(str == source);

        var source2 = AesUtils.Decrypt(AesUtils.Encrypt(str));
        Assert.IsTrue(str == source2);

        var source3 = AesUtils.Decrypt(AesUtils.Encrypt(str, key, "123", FillType.Right), key, "123", FillType.Right);
        Assert.IsTrue(str == source3);

        Assert.ThrowsException<ArgumentException>(() => AesUtils.Encrypt(str, key, "123", FillType.NoFile));

        string encryptResult = AesUtils.Encrypt(str, key, "123", FillType.Right);
        Assert.ThrowsException<ArgumentException>(() => AesUtils.Decrypt(encryptResult, key, "123", FillType.NoFile));
    }
}

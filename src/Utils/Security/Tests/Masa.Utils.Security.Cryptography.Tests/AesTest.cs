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

    [TestMethod]
    public void EncryptAndDecryptToBytes()
    {
        var str = "Hello MASA Stack";
        var encoding = Encoding.UTF8;
        string key = "12345678901234567890123456789021";
        var path = Path.Combine("1.txt");
        File.WriteAllText(path, str);
        var fileStream = File.OpenRead(path);
        var encryptBuffer = AesUtils.EncryptToBytes(fileStream, key);
        var decryptBuffer = AesUtils.DecryptToBytes(new MemoryStream(encryptBuffer), key);

        var actualResult = encoding.GetString(decryptBuffer);
        Assert.AreEqual(str, actualResult);
    }

    [TestMethod]
    public void EncryptAndDecryptFile()
    {
        var str = "Hello MASA Stack";
        var sourcePath = $"{Guid.NewGuid()}.txt";
        File.WriteAllText(sourcePath, str);
        var fileStream = File.OpenRead(sourcePath);

        var encryptPath = $"{Guid.NewGuid()}.txt";
        AesUtils.EncryptFile(fileStream, encryptPath);

        var decryptPath = $"{Guid.NewGuid()}.txt";
        AesUtils.DecryptFile(File.OpenRead(encryptPath), decryptPath);
        var actualResult = File.ReadAllText(decryptPath);
        Assert.AreEqual(str, actualResult);
    }
}

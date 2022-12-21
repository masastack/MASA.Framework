// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography.Tests;

[TestClass]
// ReSharper disable once InconsistentNaming
public class MD5UtilsTest
{
    [TestMethod]
    public void TestEncrypt()
    {
        var str = "Hello MASA Stack";
        var encryptResult = MD5Utils.Encrypt(str);
        Assert.AreEqual("e7b1bf81bacd21f9396bdbab6d881fe2", encryptResult);
    }

    [TestMethod]
    public void TestEncryptRepeat()
    {
        var str = "Hello MASA Stack";
        var encryptResult = MD5Utils.EncryptRepeat(str, 1);
        Assert.AreEqual("e7b1bf81bacd21f9396bdbab6d881fe2", encryptResult);

        encryptResult = MD5Utils.EncryptRepeat(str, 2);
        Assert.AreEqual("58349bb94668d886b6fcf3de0ccd5382", encryptResult);
    }

    [TestMethod]
    public void TestEncryptStream()
    {
        var str = "Hello MASA Stack";
        var path = Path.Combine($"{Guid.NewGuid()}.txt");
        File.WriteAllText(path, str);
        var fileStream = File.OpenRead(path);

        var encryptResult = MD5Utils.EncryptStream(fileStream);
        Assert.AreEqual("e7b1bf81bacd21f9396bdbab6d881fe2", encryptResult);
    }
}

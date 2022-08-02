using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography.Tests
{
    [TestClass]
    public class DesTest
    {
        [TestMethod]
        public void EncryptAndDecrypt()
        {
            for (int i = 0; i < 10000; i++)
            {
                string str = new Random().Next(0, 1000000000).ToString();
                string key = "masastac";
                string iv = "masastack";
                var source = DesUtils.Decrypt(DesUtils.Encrypt(str, key), key);
                var source2 = DesUtils.Decrypt(DesUtils.Encrypt(str, key, iv), key, iv);
                var source3 = DesUtils.Decrypt(DesUtils.Encrypt(str, DESEncryType.Normal), DESEncryType.Normal);

                Assert.IsTrue(str == source);
                Assert.IsTrue(str == source2);
                Assert.IsTrue(str == source3);
            }
        }
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Masa.Contrib.StackSdks.Tsc.OpenTelemetry.Tests.Extensions
{
    [TestClass]
    public class SteamTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [TestMethod]
        [DataRow("曾经沧海难为水，除却巫山不是云")]
        [DataRow("柳州柳刺史，种柳柳江边。", "gbk")]
        [DataRow("")]
        public async Task EncodingTest(string text, string? charCode = default)
        {
            Encoding? coding = null;
            byte[] bytes;
            if (!string.IsNullOrEmpty(charCode))
            {
                coding = Encoding.GetEncoding(charCode);
                bytes = coding.GetBytes(text);
            }
            else
            {
                bytes = Encoding.UTF8.GetBytes(text);
            }

            using var ms = new MemoryStream(bytes);

            (long _, string? str) = await ms.ReadAsStringAsync(encoding: coding, bufferSize: 16);

            Debug.WriteLine(str);

            if (string.IsNullOrEmpty(text))
                Assert.IsNull(str);
            else
                Assert.AreEqual(str, text);
        }

        [TestMethod]
        public async Task NullStreamReadTest()
        {
            (long _, string? text) = await default(Stream)!.ReadAsStringAsync();
            Assert.IsNull(text);
        }
    }
}

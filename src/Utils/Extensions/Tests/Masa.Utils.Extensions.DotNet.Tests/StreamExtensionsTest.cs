// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DotNet.Tests;

[TestClass]
public class StreamExtensionsTest
{
    [TestMethod]
    public void TestConvertToBytes()
    {
        var str = "Hello MASA Stack";
        var dataBuffer = str.ConvertToBytes(Encoding.UTF8);
        var memory = new MemoryStream(dataBuffer);
        var actualBuffer = memory.ConvertToBytes();
        Assert.AreEqual(str, actualBuffer.ConvertToString(Encoding.UTF8));
    }

    [TestMethod]
    public async Task TestConvertToBytesAsync()
    {
        var str = "Hello MASA Stack";
        var dataBuffer = str.ConvertToBytes(Encoding.UTF8);
        var memory = new MemoryStream(dataBuffer);
        var actualBuffer = await memory.ConvertToBytesAsync();
        Assert.AreEqual(str, actualBuffer.ConvertToString(Encoding.UTF8));
    }
}

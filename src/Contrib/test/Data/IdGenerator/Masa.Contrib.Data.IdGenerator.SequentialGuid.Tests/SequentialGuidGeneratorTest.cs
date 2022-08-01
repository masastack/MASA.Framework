// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.SequentialGuid.Tests;

[TestClass]
public class SequentialGuidGeneratorTest
{
    [TestMethod]
    public void Test()
    {
        int count = 10000000;
        List<Guid> guids = new();
        for (int i = 0; i < count; i++)
        {
            guids.Add(new SequentialGuidGenerator(SequentialGuidType.SequentialAsString).NewId());
        }
        Assert.IsTrue(guids.Count == guids.Distinct().Count());
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.SequentialGuid.Tests;

[TestClass]
public class SequentialGuidGeneratorTest
{
    [TestMethod]
    public void TestNewId()
    {
        int count = 10000000;
        List<Guid> guids = new();
        for (int i = 0; i < count; i++)
        {
            guids.Add(new SequentialGuidGenerator(SequentialGuidType.SequentialAsString).NewId());
        }
        Assert.IsTrue(guids.Count == guids.Distinct().Count());
    }

    [TestMethod]
    public void TestNewId2()
    {
        var services = new ServiceCollection();
        services.AddSequentialGuidGenerator();
        var serviceProvider = services.BuildServiceProvider();

        var idGenerator = serviceProvider.GetRequiredService<IIdGenerator<Guid>>();
        Assert.AreNotEqual(Guid.Empty, idGenerator.NewId());
        Assert.IsTrue(idGenerator.GetType() == typeof(SequentialGuidGenerator));

        var idGeneratorFactory = serviceProvider.GetService<IIdGeneratorFactory>();
        Assert.IsNotNull(idGeneratorFactory);

        var sequentialGuidGenerator = idGeneratorFactory.SequentialGuidGenerator;
        Assert.IsNotNull(sequentialGuidGenerator);
        Assert.IsTrue(sequentialGuidGenerator.GetType() == typeof(SequentialGuidGenerator));

        Assert.AreNotEqual(Guid.Empty, sequentialGuidGenerator.NewId());
    }
}

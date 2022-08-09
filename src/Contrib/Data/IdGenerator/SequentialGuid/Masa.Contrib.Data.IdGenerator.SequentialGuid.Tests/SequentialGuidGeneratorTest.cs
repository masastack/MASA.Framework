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
    public void TestSequentialGuidReturnIdGeneratorIsNotNull()
    {
        var services = new ServiceCollection();
        services.AddSequentialGuidGenerator(SequentialGuidType.SequentialAtEnd);
        var serviceProvider = services.BuildServiceProvider();
        var idGenerator = serviceProvider.GetService<IIdGenerator<Guid>>();
        Assert.IsNotNull(idGenerator);
        Assert.IsTrue(idGenerator.GetType() == typeof(SequentialGuidGenerator));

        Assert.IsNotNull(serviceProvider.GetService<IIdGenerator>());
    }

    [TestMethod]
    public void TestSequentialGuidByMasaAppReturnIdGeneratorIsNotNull()
    {
        var services = new ServiceCollection();
        MasaApp.Services = services;
        services.AddSequentialGuidGenerator(SequentialGuidType.SequentialAsString);

        var idGenerator = MasaApp.GetService<IIdGenerator<Guid>>();
        Assert.IsNotNull(idGenerator);
        Assert.IsTrue(idGenerator.GetType() == typeof(SequentialGuidGenerator));

        Assert.IsNotNull(MasaApp.GetService<IIdGenerator>());
    }
}

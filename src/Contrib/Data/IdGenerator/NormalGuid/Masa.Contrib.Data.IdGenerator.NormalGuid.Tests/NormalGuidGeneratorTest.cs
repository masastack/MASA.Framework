// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.NormalGuid.Tests;

[TestClass]
public class NormalGuidGeneratorTest
{
    [TestMethod]
    public void TestNormalGuidReturnIdGeneratorIsNotNull()
    {
        var services = new ServiceCollection();
        services.AddSimpleGuidGenerator();
        var serviceProvider = services.BuildServiceProvider();
        var idGenerator = serviceProvider.GetService<IIdGenerator<Guid>>();
        Assert.IsNotNull(idGenerator);
        Assert.IsTrue(idGenerator.GetType() == typeof(NormalGuidGenerator));

        Assert.IsNotNull(serviceProvider.GetService<IIdGenerator>());

        Assert.IsTrue(IdGeneratorFactory.GuidGenerator.GetType() == typeof(NormalGuidGenerator));
    }

    [TestMethod]
    public void TestNormalGuidByCustomNameReturnIdGeneratorIsNotNull()
    {
        var services = new ServiceCollection();
        services.AddSimpleGuidGenerator();
        var serviceProvider = services.BuildServiceProvider();
        var idGeneratorFactory = serviceProvider.GetService<IIdGeneratorFactory>();
        Assert.IsNotNull(idGeneratorFactory);

        var idGenerator = idGeneratorFactory.GuidGenerator;
        Assert.IsNotNull(idGenerator);
        Assert.IsTrue(idGenerator.GetType() == typeof(NormalGuidGenerator));

        Assert.IsTrue(IdGeneratorFactory.GuidGenerator.GetType() == typeof(NormalGuidGenerator));
    }
}

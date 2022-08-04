// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Contracts.EFCore.Tests;

[TestClass]
public class DataFilterTest
{
    private IServiceCollection _services;
    private IDataFilter _dataFilter;

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
        _services.AddSingleton<IDataFilter, DataFilter>();
        _services.AddSingleton(typeof(DataFilter<>));
        _dataFilter = new DataFilter(_services.BuildServiceProvider());
    }

    [TestMethod]
    public void TestDataFilterReturnTrue()
    {
        Assert.IsTrue(_dataFilter.IsEnabled<ISoftDelete>());

        using (_dataFilter.Disable<ISoftDelete>())
        {
            Assert.IsFalse(_dataFilter.IsEnabled<ISoftDelete>());
        }

        Assert.IsTrue(_dataFilter.IsEnabled<ISoftDelete>());
    }

    [TestMethod]
    public void TestDataFilterReturnFalse()
    {
        _dataFilter.Disable<ISoftDelete>();
        Assert.IsFalse(_dataFilter.IsEnabled<ISoftDelete>());

        using (_dataFilter.Enable<ISoftDelete>())
        {
            Assert.IsTrue(_dataFilter.IsEnabled<ISoftDelete>());
        }

        Assert.IsFalse(_dataFilter.IsEnabled<ISoftDelete>());
    }
}

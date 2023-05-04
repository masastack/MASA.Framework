// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests;

[TestClass]
public class DefaultTypeAndDefaultValueProviderTest
{
    [TestMethod]
    public void TestDefaultValues()
    {
        var defaultValueProvider = new DefaultTypeAndDefaultValueProvider();
        defaultValueProvider.TryAdd(typeof(short));
        defaultValueProvider.TryAdd(typeof(short?));
        defaultValueProvider.TryAdd(typeof(int));
        defaultValueProvider.TryAdd(typeof(int?));
        defaultValueProvider.TryAdd(typeof(long));
        defaultValueProvider.TryAdd(typeof(long?));
        defaultValueProvider.TryAdd(typeof(float));
        defaultValueProvider.TryAdd(typeof(float?));
        defaultValueProvider.TryAdd(typeof(decimal));
        defaultValueProvider.TryAdd(typeof(decimal?));
        defaultValueProvider.TryAdd(typeof(double));
        defaultValueProvider.TryAdd(typeof(double?));
        defaultValueProvider.TryAdd(typeof(Guid));
        defaultValueProvider.TryAdd(typeof(Guid?));
        defaultValueProvider.TryAdd(typeof(DateTime));
        defaultValueProvider.TryAdd(typeof(DateTime?));
        defaultValueProvider.TryAdd(typeof(string));
        defaultValueProvider.TryAdd(typeof(bool));
        defaultValueProvider.TryAdd(typeof(bool?));

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(short), out var defaultValue));
        Assert.AreEqual("0", defaultValue);

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(short?), out defaultValue));
        Assert.AreEqual(null, defaultValue);

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(int), out defaultValue));
        Assert.AreEqual("0", defaultValue);

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(int?), out defaultValue));
        Assert.AreEqual(null, defaultValue);

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(long), out defaultValue));
        Assert.AreEqual("0", defaultValue);

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(long?), out defaultValue));
        Assert.AreEqual(null, defaultValue);

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(float), out defaultValue));
        Assert.AreEqual("0", defaultValue);

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(float?), out defaultValue));
        Assert.AreEqual(null, defaultValue);

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(decimal), out defaultValue));
        Assert.AreEqual("0", defaultValue);

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(decimal?), out defaultValue));
        Assert.AreEqual(null, defaultValue);

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(double), out defaultValue));
        Assert.AreEqual("0", defaultValue);

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(double?), out defaultValue));
        Assert.AreEqual(null, defaultValue);

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(Guid), out defaultValue));
        Assert.AreEqual(Guid.Empty.ToString(), defaultValue);

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(Guid?), out defaultValue));
        Assert.AreEqual(null, defaultValue);

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(string), out defaultValue));
        Assert.AreEqual(null, defaultValue);

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(bool), out defaultValue));
        Assert.AreEqual(default(bool).ToString(), defaultValue);

        Assert.AreEqual(true, defaultValueProvider.TryGet(typeof(bool?), out defaultValue));
        Assert.AreEqual(null, defaultValue);

    }
}

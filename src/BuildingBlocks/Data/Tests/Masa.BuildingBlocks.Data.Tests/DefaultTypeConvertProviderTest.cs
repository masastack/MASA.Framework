// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.Tests;

[TestClass]
public class DefaultTypeConvertProviderTest
{
    [TestMethod]
    public void ConvertToByBaseType()
    {
        var defaultTypeConvertProvider = new DefaultTypeConvertProvider();
        Assert.AreEqual((short)0, defaultTypeConvertProvider.ConvertTo<short>("0"));
        Assert.AreEqual((short)0, defaultTypeConvertProvider.ConvertTo<short?>("0"));
        Assert.AreEqual(null, defaultTypeConvertProvider.ConvertTo<short?>(""));
        Assert.AreEqual(0, defaultTypeConvertProvider.ConvertTo<short>(""));
        Assert.AreEqual(0, defaultTypeConvertProvider.ConvertTo<int>("0"));
        Assert.AreEqual(0, defaultTypeConvertProvider.ConvertTo<int?>("0"));
        Assert.AreEqual(null, defaultTypeConvertProvider.ConvertTo<int?>(""));
        Assert.AreEqual(0, defaultTypeConvertProvider.ConvertTo<int>(""));
        Assert.AreEqual(0, defaultTypeConvertProvider.ConvertTo<long>("0"));
        Assert.AreEqual(0, defaultTypeConvertProvider.ConvertTo<long?>("0"));
        Assert.AreEqual(null, defaultTypeConvertProvider.ConvertTo<long?>(""));
        Assert.AreEqual(0, defaultTypeConvertProvider.ConvertTo<long>(""));
        Assert.AreEqual(0, defaultTypeConvertProvider.ConvertTo<float>("0"));
        Assert.AreEqual((float)0, defaultTypeConvertProvider.ConvertTo<float?>("0"));
        Assert.AreEqual(0, defaultTypeConvertProvider.ConvertTo<float>(""));
        Assert.AreEqual(null, defaultTypeConvertProvider.ConvertTo<float?>(""));
        Assert.AreEqual(0, defaultTypeConvertProvider.ConvertTo<decimal>("0"));
        Assert.AreEqual(0, defaultTypeConvertProvider.ConvertTo<decimal?>("0"));
        Assert.AreEqual(0, defaultTypeConvertProvider.ConvertTo<decimal>(""));
        Assert.AreEqual(null, defaultTypeConvertProvider.ConvertTo<decimal?>(""));
        Assert.AreEqual(0, defaultTypeConvertProvider.ConvertTo<double>("0"));
        Assert.AreEqual((double)0, defaultTypeConvertProvider.ConvertTo<double?>("0"));
        Assert.AreEqual(0, defaultTypeConvertProvider.ConvertTo<double>(""));
        Assert.AreEqual(null, defaultTypeConvertProvider.ConvertTo<double?>(""));
        Assert.AreEqual(default,
            defaultTypeConvertProvider.ConvertTo<DateTime>(default(DateTime).ToString(CultureInfo.InvariantCulture)));
        Assert.AreEqual(default(DateTime),
            defaultTypeConvertProvider.ConvertTo<DateTime?>(default(DateTime).ToString(CultureInfo.InvariantCulture)));
        Assert.AreEqual(null, defaultTypeConvertProvider.ConvertTo<DateTime?>(""));
        Assert.AreEqual(default, defaultTypeConvertProvider.ConvertTo<DateTime>(""));
        Assert.AreEqual(null, defaultTypeConvertProvider.ConvertTo<string?>(""));
        Assert.AreEqual(null, defaultTypeConvertProvider.ConvertTo<string>(""));
        Assert.AreEqual(bool.Parse("False"), defaultTypeConvertProvider.ConvertTo<bool>("False"));
        Assert.AreEqual(bool.Parse("False"), defaultTypeConvertProvider.ConvertTo<bool?>("False"));
        Assert.AreEqual(false, defaultTypeConvertProvider.ConvertTo<bool>(""));
        Assert.AreEqual(null, defaultTypeConvertProvider.ConvertTo<bool?>(""));

        var guid = Guid.NewGuid();

        Assert.AreEqual(guid, defaultTypeConvertProvider.ConvertTo<Guid>(guid.ToString()));
        Assert.AreEqual(guid, defaultTypeConvertProvider.ConvertTo<Guid?>(guid.ToString()));
        Assert.AreEqual(Guid.Empty, defaultTypeConvertProvider.ConvertTo<Guid>(""));
        Assert.AreEqual(null, defaultTypeConvertProvider.ConvertTo<Guid?>(""));

        var date = DateTime.UtcNow;

        Assert.AreEqual(date.ToString(CultureInfo.InvariantCulture),
            defaultTypeConvertProvider.ConvertTo<DateTime>(date.ToString(CultureInfo.InvariantCulture))
                .ToString(CultureInfo.InvariantCulture));

        var convertToValue = defaultTypeConvertProvider.ConvertTo<DateTime?>(date.ToString(CultureInfo.InvariantCulture));
        Assert.IsNotNull(convertToValue);
        Assert.AreEqual(date.ToString(CultureInfo.InvariantCulture), ((DateTime)convertToValue).ToString(CultureInfo.InvariantCulture));
        Assert.AreEqual(default(DateTime), defaultTypeConvertProvider.ConvertTo<DateTime>(""));
        Assert.AreEqual(null, defaultTypeConvertProvider.ConvertTo<DateTime?>(""));
    }

    [TestMethod]
    public void ConvertToByComplexType()
    {
        var list = new List<int>()
        {
            1,
            3,
            5
        };
        var value = list.ToJson();
        var defaultTypeConvertProvider = new DefaultTypeConvertProvider();
        var actualValue = defaultTypeConvertProvider.ConvertTo<List<int>>(value, new DefaultJsonDeserializer());
        Assert.IsNotNull(actualValue);
        Assert.AreEqual(3, actualValue.Count);
        Assert.AreEqual(1, actualValue[0]);
        Assert.AreEqual(3, actualValue[1]);
        Assert.AreEqual(5, actualValue[2]);
    }
}

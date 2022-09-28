// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Serialization.Json.Tests;

[TestClass]
public class JsonTest
{
    [TestMethod]
    public void TestSerialize()
    {
        var user = new User(1, "John");
        var json = new DefaultJsonSerializer().Serialize(user);
        Assert.AreEqual("{\"Id\":1,\"Name\":\"John\"}", json);
    }

    [TestMethod]
    public void TestDeserialize()
    {
        var json = "{\"Id\":1,\"Name\":\"John\"}";
        var user = new DefaultJsonDeserializer().Deserialize<User>(json);
        Assert.IsNotNull(user);
        Assert.AreEqual(1, user.Id);
        Assert.AreEqual("John", user.Name);

        user = new DefaultJsonDeserializer().Deserialize(json, typeof(User)) as User;
        Assert.IsNotNull(user);
        Assert.AreEqual(1, user.Id);
        Assert.AreEqual("John", user.Name);
    }

    [TestMethod]
    public void TestAddJsonReturnNotNull()
    {
        var services = new ServiceCollection();
        services.AddJson();
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<ISerializer>());
        Assert.IsNotNull(serviceProvider.GetService<IDeserializer>());

        var serializer = serviceProvider.GetService<ISerializer>();
        Assert.IsNotNull(serializer);
        Assert.IsTrue(serializer.GetType() == typeof(DefaultJsonSerializer));

        var deserializer = serviceProvider.GetService<IDeserializer>();
        Assert.IsNotNull(deserializer);
        Assert.IsTrue(deserializer.GetType() == typeof(DefaultJsonDeserializer));
    }

    [TestMethod]
    public void TestAddJsonByUseMasaAppReturnNotNull()
    {
        var services = new ServiceCollection();
        MasaApp.SetServiceCollection(services);
        services.AddJson();
        MasaApp.Build();
        Assert.IsNotNull(MasaApp.GetService<ISerializer>());
        Assert.IsNotNull(MasaApp.GetService<IDeserializer>());

        var serializer = MasaApp.GetService<ISerializer>();
        Assert.IsNotNull(serializer);
        Assert.IsTrue(serializer.GetType() == typeof(DefaultJsonSerializer));

        var deserializer = MasaApp.GetService<IDeserializer>();
        Assert.IsNotNull(deserializer);
        Assert.IsTrue(deserializer.GetType() == typeof(DefaultJsonDeserializer));
    }

    [TestMethod]
    public void TestAddMultiJsonReturnCountIs1()
    {
        var services = new ServiceCollection();
        services.AddJson(option =>
        {
            option.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
        }).AddJson();
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<IJsonSerializer>().Count() == 1);
        Assert.IsTrue(serviceProvider.GetServices<IJsonDeserializer>().Count() == 1);
    }

    [TestMethod]
    public void TestAddMultiJson2()
    {
        var services = new ServiceCollection();
        services.AddJson("test", options =>
        {
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        }).AddJson("test");
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<IJsonSerializer>().Count() == 1);

        var user = new User(1, null!);
        var json = serviceProvider.GetRequiredService<ISerializerFactory>().Create("test").Serialize(user);
        Assert.AreEqual("{\"Id\":" + 1 + "}", json);

        user = serviceProvider.GetRequiredService<IDeserializerFactory>().Create("test").Deserialize(json, typeof(User)) as User;
        Assert.IsNotNull(user);

        Assert.AreEqual(1, user.Id);
        Assert.IsNull(user.Name);
    }
}

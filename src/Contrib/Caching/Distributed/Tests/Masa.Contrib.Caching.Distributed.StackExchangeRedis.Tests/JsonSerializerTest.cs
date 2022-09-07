// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Dynamic;

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis.Tests;

[TestClass]
public class JsonSerializerTest
{
    [TestMethod]
    public void TestEnableDynamicTypes()
    {
        JsonSerializerOptions jsonSerializerOptions = null!;
        Assert.ThrowsException<ArgumentNullException>(() => jsonSerializerOptions.EnableDynamicTypes());
    }

    [TestMethod]
    public void TestEnableDynamicTypes2()
    {
        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.EnableDynamicTypes();

        var user = new UserModel()
        {
            Id = Guid.NewGuid(),
            Name = "test",
            Age = 18,
            Age2 = null,
            Gender = false,
            Gender2 = null,
            Tags = new List<string>()
            {
                "music"
            },
            UserRoles = new int[]
            {
                1
            },
            UserClaimType = UserClaimType.Customize,
            Avatar = new
            {
                Host = "https://www.masastack.com/",
                Path = "cover.png"
            },
            CreateTime = DateTime.Parse("2022-09-07 00:00:00")
        };
        var json = JsonSerializer.Serialize(user, jsonSerializerOptions);
        dynamic? newUser = JsonSerializer.Deserialize<ExpandoObject>(json, jsonSerializerOptions);
        Assert.IsNotNull(newUser);

        Assert.AreEqual("test", newUser!.Name);
        Assert.AreEqual(user.Id.ToString(), newUser.Id + "");
        Assert.AreEqual(18, (int)newUser.Age);
        Assert.AreEqual(null, (int?)newUser.Age2);
        Assert.AreEqual(false, newUser.Gender);
        Assert.AreEqual(null, (bool?)newUser.Gender2);
        Assert.AreEqual("https://www.masastack.com/", newUser.Avatar.Host);
        Assert.AreEqual("cover.png", newUser.Avatar.Path);
        Assert.AreEqual((int)UserClaimType.Customize, (int)newUser.UserClaimType);
        Assert.AreEqual(DateTime.Parse("2022-09-07 00:00:00"), (DateTime)newUser.CreateTime);
    }
}

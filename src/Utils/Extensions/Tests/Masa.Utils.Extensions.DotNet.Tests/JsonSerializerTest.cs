// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DotNet.Tests;

[TestClass]
public class JsonSerializerTest
{
    private enum MyCustomEnum
    {
        FortyTwo = 42,
        Hello = 77
    }

    public static DateTime MyDateTime => new DateTime(2020, 7, 8);
    public static Guid MyGuid => new("ed957609-cdfe-412f-88c1-02daca1b4f51");

    private const string DYNAMIC_TESTS_JSON =
        "{\"MyString\":\"Hello\",\"MyNull\":null,\"MyBoolean\":true,\"MyArray\":[1,2],\"MyInt\":42,\"MyDateTime\":\"2020-07-08T00:00:00\",\"MyGuid\":\"ed957609-cdfe-412f-88c1-02daca1b4f51\",\"MyObject\":{\"MyString\":\"World\"}}";

    [TestMethod]
    public void TestEnableDynamicTypes()
    {
        JsonSerializerOptions jsonSerializerOptions = null!;
        Assert.ThrowsExactly<ArgumentNullException>(() => jsonSerializerOptions.EnableDynamicTypes());
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public void TestEnableDynamicTypes2(bool ignoreNullValues)
    {
        var jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.EnableDynamicTypes();
        if (ignoreNullValues)
        {
            jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        }

        var avatar = new
        {
            Host = "https://www.masastack.com/",
            Path = "cover.png"
        };
        var user = new User()
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
            UserRoles = new[]
            {
                1
            },
            UserClaimType = UserClaimType.Customize,
            Avatar = avatar,
            Avatar2 = avatar,
            CreateTime = DateTime.Parse("2022-09-07 00:00:00")
        };
        var json = JsonSerializer.Serialize(user, jsonSerializerOptions);
        dynamic? newUser = JsonSerializer.Deserialize<ExpandoObject>(json, jsonSerializerOptions); //.ConvertToDynamic();

        Assert.IsNotNull(newUser);

        Assert.AreEqual("test", newUser!.name);
        Assert.AreEqual(user.Id.ToString(), newUser.Id + "");
        Assert.AreEqual(18, (int)newUser.Age);
        Assert.AreEqual(false, newUser.Gender);

        if (ignoreNullValues)
        {
            Assert.ThrowsExactly<RuntimeBinderException>(() => newUser.Age2);
            Assert.ThrowsExactly<RuntimeBinderException>(() => newUser.Gender2);
        }
        else
        {
            Assert.AreEqual(null, (bool?)newUser.Gender2);
            Assert.AreEqual(null, (int?)newUser.Age2);
        }

        Assert.AreEqual(1, newUser.Tags.Count);
        Assert.AreEqual("music", newUser.Tags[0]);
        Assert.AreEqual(1, newUser.UserRoles.Count);
        Assert.AreEqual(1, (int)newUser.UserRoles[0]);
        Assert.AreEqual("https://www.masastack.com/", newUser.Avatar.Host);
        Assert.AreEqual("cover.png", newUser.Avatar.Path);
        Assert.AreEqual("https://www.masastack.com/", newUser.Avatar2.Host);
        Assert.AreEqual("cover.png", newUser.Avatar2.Path);
        Assert.AreEqual((int)UserClaimType.Customize, (int)newUser.UserClaimType);
        Assert.AreEqual(DateTime.Parse("2022-09-07 00:00:00"), (DateTime)newUser.CreateTime);
    }

    [TestMethod]
    public void DynamicObject_MissingProperty()
    {
        var options = new JsonSerializerOptions();
        options.EnableDynamicTypes();
        dynamic obj = JsonSerializer.Deserialize<dynamic>("{}", options)!;

        Assert.IsNotNull(obj);
        Assert.AreEqual(null, obj.NonExistingProperty);
    }

    [TestMethod]
    public void DynamicObject_CaseSensitivity()
    {
        var options = new JsonSerializerOptions();
        options.EnableDynamicTypes();
        dynamic obj = JsonSerializer.Deserialize<dynamic>("{\"MyProperty\":42}", options)!;

        Assert.AreEqual(42, (int)obj.MyProperty);
        Assert.IsNull(obj.myproperty);
        Assert.IsNull(obj.MYPROPERTY);

        options = new JsonSerializerOptions();
        options.EnableDynamicTypes();
        options.PropertyNameCaseInsensitive = true;
        obj = JsonSerializer.Deserialize<dynamic>("{\"MyProperty\":42}", options)!;

        Assert.AreEqual(42, (int)obj.MyProperty);
        Assert.AreEqual(42, (int)obj.myproperty);
        Assert.AreEqual(42, (int)obj.MYPROPERTY);
    }

    [TestMethod]
    public void NullHandling()
    {
        var options = new JsonSerializerOptions();
        options.EnableDynamicTypes();

        dynamic? obj = JsonSerializer.Deserialize<dynamic>("null", options);
        Assert.IsNull(obj);
    }

    [TestMethod]
    public void QuotedNumbers_Deserialize()
    {
        var options = new JsonSerializerOptions();
        options.EnableDynamicTypes();
        options.NumberHandling = JsonNumberHandling.AllowReadingFromString |
            JsonNumberHandling.AllowNamedFloatingPointLiterals;

        dynamic obj = JsonSerializer.Deserialize<dynamic>("\"42\"", options)!;
        Assert.IsTrue(typeof(JsonSerializerExtensions.JsonDynamicString) == obj.GetType()!);

        Assert.AreEqual(42, (int)obj);

        obj = JsonSerializer.Deserialize<dynamic>("\"NaN\"", options)!;
        Assert.IsTrue(typeof(JsonSerializerExtensions.JsonDynamicString) == obj.GetType()!);
        Assert.AreEqual(double.NaN, (double)obj);
        Assert.AreEqual(float.NaN, (float)obj);
    }

    [TestMethod]
    public void QuotedNumbers_Serialize()
    {
        var options = new JsonSerializerOptions();
        options.EnableDynamicTypes();
        options.NumberHandling = JsonNumberHandling.WriteAsString;

        dynamic obj = 42L;
        string json = JsonSerializer.Serialize<dynamic>(obj, options);
        Assert.AreEqual("\"42\"", json);

        obj = double.NaN;
        json = JsonSerializer.Serialize<dynamic>(obj, options);
        Assert.AreEqual("\"NaN\"", json);
    }

    [TestMethod]
    public void JsonDynamicTypes_Deserialize_AsObject()
    {
        var options = new JsonSerializerOptions();
        options.EnableDynamicTypes();

        Assert.IsInstanceOfType(JsonSerializer.Deserialize<object>("[]", options), typeof(JsonSerializerExtensions.JsonDynamicArray));
        Assert.IsInstanceOfType(JsonSerializer.Deserialize<object>("true", options), typeof(JsonSerializerExtensions.JsonDynamicBoolean));
        Assert.IsInstanceOfType(JsonSerializer.Deserialize<object>("0", options), typeof(JsonSerializerExtensions.JsonDynamicNumber));
        Assert.IsInstanceOfType(JsonSerializer.Deserialize<object>("1.2", options), typeof(JsonSerializerExtensions.JsonDynamicNumber));
        Assert.IsInstanceOfType(JsonSerializer.Deserialize<object>("{}", options), typeof(JsonSerializerExtensions.JsonDynamicObject));
        Assert.IsInstanceOfType(JsonSerializer.Deserialize<object>("\"str\"", options), typeof(JsonSerializerExtensions.JsonDynamicString));
    }

    [TestMethod]
    public void JsonDynamicTypesDeserialize()
    {
        var options = new JsonSerializerOptions();
        options.EnableDynamicTypes();

        dynamic? result = JsonSerializer.Deserialize<JsonSerializerExtensions.JsonDynamicType>("{}", options);
        Assert.IsNotNull(result);
        result = JsonSerializer.Deserialize<JsonSerializerExtensions.JsonDynamicArray>("[]", options);
        Assert.IsNotNull(result);
        result = JsonSerializer.Deserialize<JsonSerializerExtensions.JsonDynamicBoolean>("true", options);
        Assert.IsNotNull(result);
        result = JsonSerializer.Deserialize<JsonSerializerExtensions.JsonDynamicNumber>("0", options);
        Assert.IsNotNull(result);
        result = JsonSerializer.Deserialize<JsonSerializerExtensions.JsonDynamicNumber>("1.2", options);
        Assert.IsNotNull(result);
        result = JsonSerializer.Deserialize<JsonSerializerExtensions.JsonDynamicObject>("{}", options);
        Assert.IsNotNull(result);
        result = JsonSerializer.Deserialize<JsonSerializerExtensions.JsonDynamicString>("\"str\"", options);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void VerifyArray()
    {
        var options = new JsonSerializerOptions();
        options.EnableDynamicTypes();
        options.Converters.Add(new JsonStringEnumConverter());

        dynamic obj = JsonSerializer.Deserialize<dynamic>(DYNAMIC_TESTS_JSON, options)!;
        Assert.IsInstanceOfType(obj, typeof(JsonSerializerExtensions.JsonDynamicObject));
        Assert.IsInstanceOfType(obj.MyArray, typeof(JsonSerializerExtensions.JsonDynamicArray));

        Assert.AreEqual(2, obj.MyArray.Count);
        Assert.AreEqual(1, (int)obj.MyArray[0]);
        Assert.AreEqual(2, (int)obj.MyArray[1]);

        // Ensure we can enumerate
        int count = 0;
        foreach (long _ in obj.MyArray)
        {
            count++;
        }

        Assert.AreEqual(2, count);

        // Ensure we can mutate through indexers
        obj.MyArray[0] = 10;
        Assert.AreEqual(10, (int)obj.MyArray[0]);
    }

    [TestMethod]
    public void VerifyPrimitives()
    {
        var options = new JsonSerializerOptions();
        options.EnableDynamicTypes();
        options.Converters.Add(new JsonStringEnumConverter());

        dynamic obj = JsonSerializer.Deserialize<dynamic>(DYNAMIC_TESTS_JSON, options)!;
        Assert.IsInstanceOfType(obj, typeof(JsonSerializerExtensions.JsonDynamicObject));

        // JsonDynamicString has an implicit cast to string.
        Assert.IsInstanceOfType(obj.MyString, typeof(JsonSerializerExtensions.JsonDynamicString));
        Assert.AreEqual("Hello", obj.MyString);

        // Verify other string-based types.
        Assert.AreEqual(MyCustomEnum.Hello, (MyCustomEnum)obj.MyString);
        Assert.AreEqual(MyDateTime, (DateTime)obj.MyDateTime);
        Assert.AreEqual(MyGuid, (Guid)obj.MyGuid);

        // JsonDynamicBoolean has an implicit cast to bool.
        Assert.IsInstanceOfType(obj.MyBoolean, typeof(JsonSerializerExtensions.JsonDynamicBoolean));
        Assert.IsTrue(obj.MyBoolean);

        // Numbers must specify the type through a cast or assignment.
        Assert.IsInstanceOfType(obj.MyInt, typeof(JsonSerializerExtensions.JsonDynamicNumber));

        bool isException = false;
        try
        {
            _ = obj.MyInt == 42L;
        }
        catch (Exception)
        {
            isException = true;
        }
        finally
        {
            Assert.IsTrue(isException);
        }

        Assert.AreEqual(42L, (long)obj.MyInt);
        Assert.AreEqual((byte)42, (byte)obj.MyInt);

        // Verify int-based Enum support through "unknown number type" fallback.
        Assert.AreEqual(MyCustomEnum.FortyTwo, (MyCustomEnum)obj.MyInt);

        // Verify floating point.
        obj = JsonSerializer.Deserialize<dynamic>("4.2", options)!;
        Assert.IsInstanceOfType(obj, typeof(JsonSerializerExtensions.JsonDynamicNumber));

        double dbl = (double)obj;
#if !BUILDING_INBOX_LIBRARY
        string temp = dbl.ToString(System.Globalization.CultureInfo.InvariantCulture);
        // The reader uses "G17" format which causes temp to be 4.2000000000000002 in this case.
        dbl = double.Parse(temp, System.Globalization.CultureInfo.InvariantCulture);
#endif
        Assert.AreEqual(4.2, dbl);
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public void TestSerializeAndDeserialize(bool ignoreNullValues)
    {
        var jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.EnableDynamicTypes();
        if (ignoreNullValues)
        {
            jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        }

        var avatar = new
        {
            Host = "https://www.masastack.com/",
            Path = "cover.png"
        };
        var user = new User()
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
            UserRoles = new[]
            {
                1
            },
            UserClaimType = UserClaimType.Customize,
            Avatar = avatar,
            Avatar2 = avatar,
            CreateTime = DateTime.Parse("2022-09-07 00:00:00")
        };
        var json = JsonSerializer.Serialize(user, jsonSerializerOptions);

        var deserializeUser = JsonSerializer.Deserialize<User>(json, jsonSerializerOptions);
        Assert.IsNotNull(deserializeUser);
        Assert.AreEqual(user.Id, deserializeUser.Id);
        Assert.AreEqual(user.Name, deserializeUser.Name);
        Assert.AreEqual(user.Age, deserializeUser.Age);
        Assert.AreEqual(user.Age2, deserializeUser.Age2);
        Assert.AreEqual(user.Gender, deserializeUser.Gender);
        Assert.AreEqual(user.Gender2, deserializeUser.Gender2);
        Assert.AreEqual(user.Tags.Count, deserializeUser.Tags.Count);
        Assert.AreEqual(user.UserRoles.Length, deserializeUser.UserRoles.Length);
        Assert.AreEqual(user.UserClaimType, deserializeUser.UserClaimType);

        Assert.AreEqual(GetPropertyValue(user, "Avatar", "Host"), GetPropertyValue(deserializeUser, "Avatar", "Host"));
        Assert.AreEqual(GetPropertyValue(user, "Avatar", "Path"), GetPropertyValue(deserializeUser, "Avatar", "Path"));

        Assert.AreEqual(user.CreateTime, deserializeUser.CreateTime);

        string? GetPropertyValue(object obj, params string[] names)
        {
            var currentObj =
                JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(JsonSerializer.Serialize(obj, jsonSerializerOptions));
            Assert.IsNotNull(currentObj);
            return currentObj[names[0]].GetProperty(names[1]).GetString();
        }
    }

    [TestMethod]
    public void TestSerializeAndDeserialize()
    {
        var jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.EnableDynamicTypes();

        var avatar = new
        {
            Host = "https://www.masastack.com/",
            Path = "cover.png"
        };
        var user = new User()
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
            UserRoles = new[]
            {
                1
            },
            UserClaimType = UserClaimType.Customize,
            Avatar = avatar,
            Avatar2 = avatar,
            CreateTime = DateTime.Parse("2022-09-07 00:00:00")
        };
        var userList = new List<User>()
        {
            user
        };

        var json = JsonSerializer.Serialize(userList, jsonSerializerOptions);

        var deserializeUserList = JsonSerializer.Deserialize<List<User>>(json, jsonSerializerOptions);
        Assert.IsNotNull(deserializeUserList);
        Assert.AreEqual(1, deserializeUserList.Count);
        var deserializeUser = deserializeUserList.FirstOrDefault();
        Assert.IsNotNull(deserializeUser);
        Assert.AreEqual(user.Id, deserializeUser.Id);
        Assert.AreEqual(user.Name, deserializeUser.Name);
        Assert.AreEqual(user.Age, deserializeUser.Age);
        Assert.AreEqual(user.Age2, deserializeUser.Age2);
        Assert.AreEqual(user.Gender, deserializeUser.Gender);
        Assert.AreEqual(user.Gender2, deserializeUser.Gender2);
        Assert.AreEqual(user.Tags.Count, deserializeUser.Tags.Count);
        Assert.AreEqual(user.UserRoles.Length, deserializeUser.UserRoles.Length);
        Assert.AreEqual(user.UserClaimType, deserializeUser.UserClaimType);

        Assert.AreEqual(GetPropertyValue(user, "Avatar", "Host"), GetPropertyValue(deserializeUser, "Avatar", "Host"));
        Assert.AreEqual(GetPropertyValue(user, "Avatar", "Path"), GetPropertyValue(deserializeUser, "Avatar", "Path"));

        Assert.AreEqual(user.CreateTime, deserializeUser.CreateTime);

        string? GetPropertyValue(object obj, params string[] names)
        {
            var currentObj =
                JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(JsonSerializer.Serialize(obj, jsonSerializerOptions));
            Assert.IsNotNull(currentObj);
            return currentObj[names[0]].GetProperty(names[1]).GetString();
        }
    }
}

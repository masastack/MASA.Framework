// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis.Tests;

[TestClass]
public class JsonSerializerTest
{
    private enum MyCustomEnum
    {
        Default = 0,
        FortyTwo = 42,
        Hello = 77
    }

    public static DateTime MyDateTime => new DateTime(2020, 7, 8);
    public static Guid MyGuid => new Guid("ed957609-cdfe-412f-88c1-02daca1b4f51");

    private const string DynamicTestsJson =
        "{\"MyString\":\"Hello\",\"MyNull\":null,\"MyBoolean\":true,\"MyArray\":[1,2],\"MyInt\":42,\"MyDateTime\":\"2020-07-08T00:00:00\",\"MyGuid\":\"ed957609-cdfe-412f-88c1-02daca1b4f51\",\"MyObject\":{\"MyString\":\"World\"}}";

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
            UserRoles = new[]
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
        Assert.AreEqual(1, newUser.Tags.Count);
        Assert.AreEqual("music", newUser.Tags[0]);
        Assert.AreEqual(1, newUser.UserRoles.Count);
        Assert.AreEqual(1, (int)newUser.UserRoles[0]);
        Assert.AreEqual("https://www.masastack.com/", newUser.Avatar.Host);
        Assert.AreEqual("cover.png", newUser.Avatar.Path);
        Assert.AreEqual((int)UserClaimType.Customize, (int)newUser.UserClaimType);
        Assert.AreEqual(DateTime.Parse("2022-09-07 00:00:00"), (DateTime)newUser.CreateTime);
    }

    [TestMethod]
    public void DynamicObject_MissingProperty()
    {
        var options = new JsonSerializerOptions();
        options.EnableDynamicTypes();
        dynamic obj = JsonSerializer.Deserialize<dynamic>("{}", options);

        Assert.IsNotNull(obj);
        Assert.AreEqual(null, obj!.NonExistingProperty);
    }

    [TestMethod]
    public void DynamicObject_CaseSensitivity()
    {
        var options = new JsonSerializerOptions();
        options.EnableDynamicTypes();
        dynamic obj = JsonSerializer.Deserialize<dynamic>("{\"MyProperty\":42}", options);

        Assert.AreEqual(42, (int)obj.MyProperty);
        Assert.IsNull(obj.myproperty);
        Assert.IsNull(obj.MYPROPERTY);

        options = new JsonSerializerOptions();
        options.EnableDynamicTypes();
        options.PropertyNameCaseInsensitive = true;
        obj = JsonSerializer.Deserialize<dynamic>("{\"MyProperty\":42}", options);

        Assert.AreEqual(42, (int)obj.MyProperty);
        Assert.AreEqual(42, (int)obj.myproperty);
        Assert.AreEqual(42, (int)obj.MYPROPERTY);
    }

    [TestMethod]
    public void NullHandling()
    {
        var options = new JsonSerializerOptions();
        options.EnableDynamicTypes();

        dynamic obj = JsonSerializer.Deserialize<dynamic>("null", options);
        Assert.IsNull(obj);
    }

    [TestMethod]
    public void QuotedNumbers_Deserialize()
    {
        var options = new JsonSerializerOptions();
        options.EnableDynamicTypes();
        options.NumberHandling = JsonNumberHandling.AllowReadingFromString |
            JsonNumberHandling.AllowNamedFloatingPointLiterals;

        dynamic obj = JsonSerializer.Deserialize<dynamic>("\"42\"", options);
        Assert.IsTrue(typeof(JsonSerializerExtensions.JsonDynamicString) == obj.GetType());

        Assert.AreEqual(42, (int)obj);

        obj = JsonSerializer.Deserialize<dynamic>("\"NaN\"", options);
        Assert.IsTrue(typeof(JsonSerializerExtensions.JsonDynamicString) == obj.GetType());
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
    public void JsonDynamicTypes_Deserialize()
    {
        var options = new JsonSerializerOptions();
        options.EnableDynamicTypes();

        JsonSerializer.Deserialize<JsonSerializerExtensions.JsonDynamicType>("{}", options);
        JsonSerializer.Deserialize<JsonSerializerExtensions.JsonDynamicArray>("[]", options);
        JsonSerializer.Deserialize<JsonSerializerExtensions.JsonDynamicBoolean>("true", options);
        JsonSerializer.Deserialize<JsonSerializerExtensions.JsonDynamicNumber>("0", options);
        JsonSerializer.Deserialize<JsonSerializerExtensions.JsonDynamicNumber>("1.2", options);
        JsonSerializer.Deserialize<JsonSerializerExtensions.JsonDynamicObject>("{}", options);
        JsonSerializer.Deserialize<JsonSerializerExtensions.JsonDynamicString>("\"str\"", options);
    }

    [TestMethod]
    public void VerifyArray()
    {
        var options = new JsonSerializerOptions();
        options.EnableDynamicTypes();
        options.Converters.Add(new JsonStringEnumConverter());

        dynamic obj = JsonSerializer.Deserialize<dynamic>(DynamicTestsJson, options);
        Assert.IsInstanceOfType(obj, typeof(JsonSerializerExtensions.JsonDynamicObject));
        Assert.IsInstanceOfType(obj.MyArray, typeof(JsonSerializerExtensions.JsonDynamicArray));

        Assert.AreEqual(2, obj.MyArray.Count);
        Assert.AreEqual(1, (int)obj.MyArray[0]);
        Assert.AreEqual(2, (int)obj.MyArray[1]);

        // Ensure we can enumerate
        int count = 0;
        foreach (long value in obj.MyArray)
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

        dynamic obj = JsonSerializer.Deserialize<dynamic>(DynamicTestsJson, options)!;
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
        obj = JsonSerializer.Deserialize<dynamic>("4.2", options);
        Assert.IsInstanceOfType(obj, typeof(JsonSerializerExtensions.JsonDynamicNumber));

        double dbl = (double)obj;
#if !BUILDING_INBOX_LIBRARY
        string temp = dbl.ToString(System.Globalization.CultureInfo.InvariantCulture);
        // The reader uses "G17" format which causes temp to be 4.2000000000000002 in this case.
        dbl = double.Parse(temp, System.Globalization.CultureInfo.InvariantCulture);
#endif
        Assert.AreEqual(4.2, dbl);
    }
}

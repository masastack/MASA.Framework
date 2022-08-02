// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Prometheus.Test;

[TestClass]
public class ObjectExtensionsTests
{
    [TestMethod]
    [DataRow(1)]
    [DataRow((byte)1)]
    [DataRow((char)2)]
    [DataRow((uint)3)]
    [DataRow((long)4)]
    [DataRow((float)5.6789)]
    [DataRow(5.6789)]
    [DataRow("string")]
    public void SampleValueTest(object value)
    {
        var result = value.ToUrlParam();
        var str = value.ToString();
        Assert.AreEqual(str, result);
    }

    [TestMethod]
    public void StructTest()
    {
        var user = new UserStruct
        {
            Name = "Bob",
            Age = 30,
            Gender = "Male"
        };

        var result = user.ToUrlParam();
        var str = $"age={user.Age}&gender={user.Gender}&name={user.Name}";
        Assert.AreEqual(str, result);

        user.Name = "王占山";
        str = $"age={user.Age}&gender={user.Gender}&name={System.Web.HttpUtility.UrlEncode(user.Name, Encoding.UTF8)}";
        result = user.ToUrlParam();
        Assert.AreEqual(str, result);
    }

    [TestMethod]
    public void ClassTest()
    {
        var obj = new
        {
            a = "test",
            d = (float)34.56,
            ch = "中文说明",
            t = ResultTypes.Scalar
        };

        var result = obj.ToUrlParam(isEnumString: true);
        var str = $"a={obj.a}&ch={System.Web.HttpUtility.UrlEncode(obj.ch, Encoding.UTF8)}&d={obj.d}&t={obj.t}";
        Assert.AreEqual(str, result);

        result = obj.ToUrlParam(isEnumString: false);
        str = $"a={obj.a}&ch={System.Web.HttpUtility.UrlEncode(obj.ch, Encoding.UTF8)}&d={obj.d}&t={(int)obj.t}";
        Assert.AreEqual(str, result);

        result = obj.ToUrlParam(isEnumString: false, isUrlEncode: false);
        str = $"a={obj.a}&ch={obj.ch}&d={obj.d}&t={(int)obj.t}";
        Assert.AreEqual(str, result);
    }

    [TestMethod]
    public void ArrayTest()
    {
        var array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var result = array.ToUrlParam(isUrlEncode: false);
        var str = string.Join("&[]=", array);
        str = $"[]={str}";
        Assert.AreEqual(str, result);
    }

    [TestMethod]
    public void ObjArrayTest()
    {
        var array = new QueryRequest[] {
            new QueryRequest{
                 Query="where1",
                  Time="2021-01-02",
                   TimeOut="5s"
            },
            new QueryRequest{
                 Query="where2",
                  Time="2021-01-01",
                   TimeOut="5s"
            }
        };

        var result = array.ToUrlParam(isUrlEncode: false);
        var str = $"[].query={array[0].Query}&[].time={array[0].Time}&[].timeOut={array[0].TimeOut}";
        str += $"&[].query={array[1].Query}&[].time={array[1].Time}&[].timeOut={array[1].TimeOut}";
        Assert.AreEqual(str, result);

        var obj = new
        {
            Values = array
        };
        result = obj.ToUrlParam(isUrlEncode: false);
        str = $"values[].query={array[0].Query}&values[].time={array[0].Time}&values[].timeOut={array[0].TimeOut}";
        str += $"&values[].query={array[1].Query}&values[].time={array[1].Time}&values[].timeOut={array[1].TimeOut}";
        Assert.AreEqual(str, result);
    }

    [TestMethod]
    public void IEnumberTest()
    {
        var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
        var result = list.ToUrlParam(isUrlEncode: false);
        var str = string.Join("&[]=", list);
        str = $"[]={str}";
        Assert.AreEqual(str, result);
    }

    [TestMethod]
    public void ObjListTest()
    {
        var array = new List<QueryRequest> {
            new QueryRequest{
                 Query="where1",
                  Time="2021-01-02",
                   TimeOut="5s"
            },
            new QueryRequest{
                 Query="where2",
                  Time="2021-01-01",
                   TimeOut="5s"
            }
        };

        var result = array.ToUrlParam(isUrlEncode: false);
        var str = $"[].query={array[0].Query}&[].time={array[0].Time}&[].timeOut={array[0].TimeOut}";
        str += $"&[].query={array[1].Query}&[].time={array[1].Time}&[].timeOut={array[1].TimeOut}";
        Assert.AreEqual(str, result);

        var obj = new
        {
            Values = array
        };
        result = obj.ToUrlParam(isUrlEncode: false);
        str = $"values[].query={array[0].Query}&values[].time={array[0].Time}&values[].timeOut={array[0].TimeOut}";
        str += $"&values[].query={array[1].Query}&values[].time={array[1].Time}&values[].timeOut={array[1].TimeOut}";
        Assert.AreEqual(str, result);
    }

    [TestMethod]
    public void KeyValueTest()
    {
        var dic = new Dictionary<string, object> {
            { "Name","David"},
            {"Age",30 },
            {"Sex","Male" }
        };

        var result = dic.ToUrlParam();
        var builder = new StringBuilder();
        var keys = dic.Keys.ToList();
        keys.Sort();
        foreach (var key in keys)
        {
            builder.Append($"&{key.ToCamelCase()}={dic[key]}");
        }
        builder.Remove(0, 1);
        Assert.AreEqual(builder.ToString(), result);
    }
}

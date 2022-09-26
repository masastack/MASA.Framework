// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Serialization.Json.Tests.Infrastructure;

public class NullToEmptyStringResolver : CamelCasePropertyNamesContractResolver
{
    /// <summary>
    /// 创建属性
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="memberSerialization">序列化成员</param>
    /// <returns></returns>
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        return type.GetProperties().Select(c =>
        {
            var jsonProperty = base.CreateProperty(c, memberSerialization);
            jsonProperty.ValueProvider = new NullToEmptyStringValueProvider(c);
            return jsonProperty;
        }).ToList();
    }
}

public class NullToEmptyStringValueProvider : IValueProvider
{
    private readonly PropertyInfo _memberInfo;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="memberInfo"></param>
    public NullToEmptyStringValueProvider(PropertyInfo memberInfo)
    {
        _memberInfo = memberInfo;
    }

    /// <summary>
    /// 获取Value
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public object GetValue(object target)
    {
        var result = _memberInfo.GetValue(target);
        if (_memberInfo.PropertyType == typeof(string) && result == null)
            result = string.Empty;
        return result;
    }

    /// <summary>
    /// 设置Value
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    public void SetValue(object target, object value)
    {
        _memberInfo.SetValue(target, value);
    }
}

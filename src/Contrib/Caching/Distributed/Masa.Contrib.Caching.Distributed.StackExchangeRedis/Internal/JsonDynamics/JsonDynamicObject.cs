// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

/// <summary>
/// Supports dynamic objects.
/// </summary>
internal sealed class JsonDynamicObject : JsonDynamicType, IDictionary<string, object>
{
    private readonly IDictionary<string, object> _value;

    public JsonDynamicObject(JsonSerializerOptions options)
        : base(options)
    {
        _value = new Dictionary<string, object>(options.PropertyNameCaseInsensitive
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal);
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (_value.TryGetValue(binder.Name, out result))
        {
            if (result is JsonDynamicObject dynamicObj)
            {
                return dynamicObj.TryConvert(binder.ReturnType, out result);
            }

            return true;
        }

        // Return null for missing properties.
        result = null;
        return true;
    }

    protected override bool TryConvert(Type returnType, out object? result)
    {
        if (returnType.IsAssignableFrom(typeof(IDictionary<string, object>)))
        {
            result = this;
            return true;
        }

        result = null;
        return false;
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        _value[binder.Name] = value!;
        return true;
    }

    internal override object Value => _value;

    public override T? GetValue<T>() where T : default => throw new NotSupportedException();
    public override void SetValue(object value) => throw new NotSupportedException();

    // IDictionary members.
    public void Add(string key, object value) => _value.Add(key, value!);
    void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item) => _value.Add(item!);
    public void Clear() => _value.Clear();
    bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item) => _value.Contains(item);
    public bool ContainsKey(string key) => _value.ContainsKey(key);

    void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        => _value.CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _value.GetEnumerator();
    public bool Remove(string key) => _value.Remove(key);
    bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item) => _value.Remove(item);

    public object this[string key]
    {
        get => _value[key];
        set => _value[key] = value!;
    }

    ICollection<string> IDictionary<string, object>.Keys => _value.Keys;
    ICollection<object> IDictionary<string, object>.Values => _value.Values;
    public int Count => _value.Count;
    bool ICollection<KeyValuePair<string, object>>.IsReadOnly => _value.IsReadOnly;
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_value).GetEnumerator();
    public bool TryGetValue(string key, out object value) => _value.TryGetValue(key, out value!);
}

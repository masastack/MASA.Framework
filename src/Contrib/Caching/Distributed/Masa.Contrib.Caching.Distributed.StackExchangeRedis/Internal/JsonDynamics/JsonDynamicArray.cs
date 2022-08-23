// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

/// <summary>
/// Supports dynamic arrays.
/// </summary>
internal sealed class JsonDynamicArray : JsonDynamicType, IList<object>
{
    private readonly IList<object> _value;

    public JsonDynamicArray(JsonSerializerOptions options) : base(options)
    {
        _value = new List<object>();
    }

    protected override bool TryConvert(Type returnType, out object? result)
    {
        if (returnType.IsAssignableFrom(typeof(IList<object>)))
        {
            result = _value;
            return true;
        }

        result = null;
        return false;
    }

    internal override object Value => _value;

    public override T? GetValue<T>() where T : default => throw new NotSupportedException();
    public override void SetValue(object value) => throw new NotSupportedException();

    // IList members.
    public object this[int index]
    {
        get => _value[index];
        set => _value[index] = value;
    }

    public int Count => _value.Count;
    bool ICollection<object>.IsReadOnly => _value.IsReadOnly;
    public void Add(object item) => _value.Add(item);
    public void Clear() => _value.Clear();
    public bool Contains(object item) => _value.Contains(item);
    void ICollection<object>.CopyTo(object[] array, int arrayIndex) => _value.CopyTo(array, arrayIndex);
    public IEnumerator<object> GetEnumerator() => _value.GetEnumerator();
    public int IndexOf(object item) => _value.IndexOf(item);
    public void Insert(int index, object item) => _value.Insert(index, item);
    public bool Remove(object item) => _value.Remove(item);
    public void RemoveAt(int index) => _value.RemoveAt(index);
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_value).GetEnumerator();
}

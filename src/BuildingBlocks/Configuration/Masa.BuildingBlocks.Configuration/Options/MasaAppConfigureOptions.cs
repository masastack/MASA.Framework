// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration.Options;

public class MasaAppConfigureOptions
{
    public string AppId { get => GetValue(nameof(AppId)); set => Data[nameof(AppId)] = value; }

    public string Environment { get => GetValue(nameof(Environment)); set => Data[nameof(Environment)] = value; }

    public string Cluster { get => GetValue(nameof(Cluster)); set => Data[nameof(Cluster)] = value; }

    private Dictionary<string, string> Data { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public int Length => Data.Count;

    public string GetValue(string key) => GetValue(key, () => string.Empty);

    public string GetValue(string key, Func<string> defaultFunc)
    {
        if (Data.ContainsKey(key)) return Data[key];

        return defaultFunc.Invoke();
    }

    public bool TryAdd(string key, string value) => Data.TryAdd(key, value);

    public void Set(string key, string value) => Data[key] = value;

    public void TryRemove(string key)
    {
        if (Data.ContainsKey(key)) Remove(key);
    }

    public bool Remove(string key) => Data.Remove(key);
}

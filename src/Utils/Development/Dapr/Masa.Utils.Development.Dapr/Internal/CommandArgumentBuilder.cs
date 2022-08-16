// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Development.Dapr.Internal;

public class CommandArgumentBuilder
{
    private const string DEFAULT_ARGUMENT_SEPARATOR = " ";
    private const string DEFAULT_ARGUMENT_PREFIX = "--";

    private readonly Dictionary<string, string> _arguments = new();

    public string ArgumemtPrefix { get; }

    private string _argumentSeparator = default!;

    public string ArgumentSeparator
    {
        get => _argumentSeparator;
        set => _argumentSeparator = string.IsNullOrEmpty(value) ? DEFAULT_ARGUMENT_SEPARATOR : value;
    }

    public CommandArgumentBuilder(string? argumentPrefix = null)
    {
        ArgumemtPrefix = string.IsNullOrEmpty(argumentPrefix) ? DEFAULT_ARGUMENT_PREFIX : argumentPrefix;
        ArgumentSeparator = DEFAULT_ARGUMENT_SEPARATOR;
    }

    public CommandArgumentBuilder Add(string name, object? value = null)
    {
        if (value == null || string.IsNullOrEmpty(name))
        {
            return this;
        }

        if (_arguments.ContainsKey(name))
        {
            _arguments.Remove(name);
        }

        _arguments.Add(name, value.ToString() ?? "");
        return this;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var item in _arguments)
        {
            stringBuilder.Append($"{ArgumentSeparator}{ArgumemtPrefix}{item.Key}{ArgumentSeparator}{item.Value}");
        }

        return stringBuilder.ToString();
    }
}

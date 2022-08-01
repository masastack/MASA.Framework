// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal.Parser;

/// <summary>
/// https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Configuration.Json/src/JsonConfigurationFileParser.cs
/// </summary>
internal sealed class JsonConfigurationParser
{
    private JsonConfigurationParser()
    {
    }

    private readonly Dictionary<string, string> _data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private readonly Stack<string> _paths = new Stack<string>();

    public static IDictionary<string, string> Parse(string json)
        => new JsonConfigurationParser().ParseJson(json);

    private IDictionary<string, string> ParseJson(string json)
    {
        var jsonDocumentOptions = new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        var doc = JsonDocument.Parse(json, jsonDocumentOptions);

        if (doc.RootElement.ValueKind != JsonValueKind.Object)
        {
            throw new FormatException($"[{doc.RootElement.ValueKind}]Invalid top level JsonElement.");
        }

        VisitElement(doc.RootElement);

        return _data;
    }

    private void VisitElement(JsonElement element)
    {
        var isEmpty = true;

        foreach (JsonProperty property in element.EnumerateObject())
        {
            isEmpty = false;
            EnterContext(property.Name);
            VisitValue(property.Value);
            ExitContext();
        }

        if (isEmpty && _paths.Count > 0)
        {
            _data[_paths.Peek()] = "";
        }
    }

    private void VisitValue(JsonElement value)
    {
        Debug.Assert(_paths.Count > 0);

        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                VisitElement(value);
                break;

            case JsonValueKind.Array:
                int index = 0;
                foreach (JsonElement arrayElement in value.EnumerateArray())
                {
                    EnterContext(index.ToString());
                    VisitValue(arrayElement);
                    ExitContext();
                    index++;
                }

                break;

            case JsonValueKind.Number:
            case JsonValueKind.String:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
                string key = _paths.Peek();
                if (_data.ContainsKey(key))
                {
                    throw new FormatException($"[{key}]key is duplicated.");
                }

                _data[key] = value.ToString();
                break;

            default:
                throw new FormatException($"[{value.ValueKind}]Unsupported json token.");
        }
    }

    private void EnterContext(string context) =>
        _paths.Push(_paths.Count > 0 ? _paths.Peek() + ConfigurationPath.KeyDelimiter + context : context);

    private void ExitContext() => _paths.Pop();
}

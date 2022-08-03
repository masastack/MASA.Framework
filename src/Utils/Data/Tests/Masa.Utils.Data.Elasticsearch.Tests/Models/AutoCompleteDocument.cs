// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Tests.Models;

public class AutoCompleteDocument<TValue> where TValue : notnull
{
    public string Id { get; set; }

    public string Text { get; set; }

    public TValue? Value { get; set; }

    public AutoCompleteDocument()
    {
    }

    public AutoCompleteDocument(string text, TValue? value)
        : this(value?.ToString() ?? throw new ArgumentException($"{value} cannot be empty", nameof(value)), text, value)
    {
    }

    public AutoCompleteDocument(string id, string text, TValue? value) : this()
    {
        Id = id;
        Text = text;
        Value = value;
    }
}


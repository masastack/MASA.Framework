// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete;

public class AutoCompleteDocument
{
    private string? _documentId;

    private string? _text;

    public string Text
    {
        get
        {
            return _text ??= GetText();
        }
        init
        {
            if (value != null!)
                _text = value;
        }
    }

    public AutoCompleteDocument()
    {
    }

    public AutoCompleteDocument(string text) : this()
    {
        Text = text;
    }

    public virtual string GetDocumentId() => _documentId ??= Guid.NewGuid().ToString();

    protected virtual string GetText() => string.Empty;
}

public class AutoCompleteDocument<TValue> : AutoCompleteDocument where TValue : notnull
{
    private readonly string _id;

    public string Id
    {
        get
        {
            if (string.IsNullOrEmpty(_id))
            {
                var val = Value.ToString();
                MasaArgumentException.ThrowIfNullOrEmpty(val, nameof(Id));
                return val;
            }

            return _id;
        }
        init
        {
            MasaArgumentException.ThrowIfNullOrEmpty(value, nameof(Id));

            _id = value;
        }
    }

    public TValue Value { get; set; }

    public AutoCompleteDocument()
    {
    }

    public AutoCompleteDocument(string text, TValue value)
        : this(value?.ToString() ?? throw new ArgumentException($"{value} cannot be empty", nameof(value)), text, value)
    {
    }

    public AutoCompleteDocument(string id, string text, TValue value) : this()
    {
        Id = id;
        Text = text;
        Value = value;
    }

    public override string GetDocumentId() => Id;
}

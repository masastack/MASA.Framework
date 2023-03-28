// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete;

public class ScopedAutoCompleteClient
{
    public IAutoCompleteClient AutoCompleteClient { get; }

    public ScopedAutoCompleteClient(IAutoCompleteClient autoCompleteClient)
    {
        AutoCompleteClient = autoCompleteClient;
    }
}

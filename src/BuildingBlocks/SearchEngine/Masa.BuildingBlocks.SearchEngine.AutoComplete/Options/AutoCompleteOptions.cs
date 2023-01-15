// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.BuildingBlocks.SearchEngine.AutoComplete;

public class AutoCompleteOptions
{
    public string Field { get; set; }

    private int _page;

    public int Page
    {
        get => _page;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, 0, nameof(Page));

            _page = value;
        }
    }

    private int _pageSize;

    public int PageSize
    {
        get => _pageSize;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, 0, nameof(PageSize));

            _pageSize = value;
        }
    }

    public SearchType? SearchType { get; }

    public AutoCompleteOptions(SearchType? searchType = null)
    {
        this.Field = "text";
        this.Page = 1;
        this.PageSize = 10;
        this.SearchType = searchType;
    }
}

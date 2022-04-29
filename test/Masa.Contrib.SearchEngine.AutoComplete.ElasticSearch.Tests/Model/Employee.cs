// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch.Tests.Model;

public class Employee : AutoCompleteDocument<int>
{
    public string Phone { get; set; }
}

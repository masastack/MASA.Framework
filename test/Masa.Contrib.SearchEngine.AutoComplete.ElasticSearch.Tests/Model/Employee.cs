// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.SearchEngine.AutoComplete.ElasticSearch.Tests.Model;

public class Employee : AutoCompleteDocument
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Phone { get; set; }

    protected override string GetText() => Name;

    public override string GetDocumentId() => Id.ToString();
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete;

public class AutoCompleteOptionsBuilder
{
    public IServiceCollection Services { get; }

    public string Name { get; }

    public Type DocumentType { get; }

    public AutoCompleteOptionsBuilder(IServiceCollection services, Type documentType)
        : this(services, Microsoft.Extensions.Options.Options.DefaultName, documentType)
    {
    }

    public AutoCompleteOptionsBuilder(IServiceCollection services, string name, Type documentType)
    {
        Services = services;
        Name = name;
        DocumentType = documentType;
    }
}

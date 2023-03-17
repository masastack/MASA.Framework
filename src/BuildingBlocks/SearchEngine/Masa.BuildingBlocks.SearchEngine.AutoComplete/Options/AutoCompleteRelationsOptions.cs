// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.SearchEngine.AutoComplete;

public class AutoCompleteRelationsOptions : MasaRelationOptions<IManualAutoCompleteClient>
{
    public AutoCompleteRelationsOptions(string name) : base(name)
    {
    }

    public AutoCompleteRelationsOptions(string name, Func<IServiceProvider, IManualAutoCompleteClient> func) : this(name)
    {
        Func = func;
    }
}

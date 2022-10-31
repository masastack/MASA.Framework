// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.BuildingBlocks.Caching;

public class TypeAliasRelationOptions : MasaRelationOptions<ITypeAliasProvider>
{
    public TypeAliasRelationOptions(string name, Func<IServiceProvider, ITypeAliasProvider> func) : base(name)
    {
        Func = func;
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.TypeConverts;

public class TypeConvertRelationOptions : MasaRelationOptions<ITypeConvertProvider>
{
    public TypeConvertRelationOptions(string name, Func<IServiceProvider, ITypeConvertProvider> func)
        : base(name)
    {
        Func = func;
    }
}

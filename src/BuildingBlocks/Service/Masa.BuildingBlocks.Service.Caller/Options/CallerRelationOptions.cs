// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller.Options;

public class CallerRelationOptions
{
    public string Name { get; } = default!;

    public bool IsDefault { get; }

    public Func<IServiceProvider, ICaller> Func { get; } = default!;

    public CallerRelationOptions(string name, bool isDefault, Func<IServiceProvider, ICaller> func)
    {
        Name = name;
        IsDefault = isDefault;
        Func = func;
    }
}

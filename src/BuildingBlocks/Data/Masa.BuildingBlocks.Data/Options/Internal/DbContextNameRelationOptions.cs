// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

internal class DbContextNameRelationOptions
{
    public string Name { get; }

    public Type DbContextType { get; }

    public DbContextNameRelationOptions(string name, Type dbContextType)
    {
        Name = name;
        DbContextType = dbContextType;
    }
}

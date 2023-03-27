// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

public class LocalMessageTableOptions
{
    public Type? DbContextType { get; set; }

    private string? _sectionName;

    public string SectionName
    {
        get
        {
            if (DbContextType == null)
                _sectionName = string.Empty;
            
            return _sectionName ??= ConnectionStringNameAttribute.GetConnStringName(DbContextType!);
        }
    }
}

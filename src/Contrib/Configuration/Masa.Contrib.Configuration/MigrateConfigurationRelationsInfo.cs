// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

public class MigrateConfigurationRelationsInfo
{
    public string SectionName { get; }

    public string OptSectionName { get; }

    public MigrateConfigurationRelationsInfo(string sectionName, string optSectionName)
    {
        SectionName = sectionName;
        OptSectionName = optSectionName;
    }
}

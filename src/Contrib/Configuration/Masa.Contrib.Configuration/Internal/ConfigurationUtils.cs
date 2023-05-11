// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal static class ConfigurationUtils
{
    public static void AddRegistrationOptions(List<ConfigurationRelationOptions> relationOptionsCollection, ConfigurationRelationOptions configurationRelations)
    {
        if (relationOptionsCollection.Any(relation =>
                relation.ParentSection == configurationRelations.ParentSection &&
                relation.SectionType == configurationRelations.SectionType &&
                relation.Section == configurationRelations.Section &&
                relation.Name == configurationRelations.Name &&
                relation.ObjectType == configurationRelations.ObjectType))
            throw new MasaException("The current section already has a configuration");

        relationOptionsCollection.Add(configurationRelations);
    }
}

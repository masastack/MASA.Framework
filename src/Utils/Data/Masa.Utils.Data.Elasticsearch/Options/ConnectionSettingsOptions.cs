// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Options;

public class ConnectionSettingsOptions
{
    public IConnection? Connection { get; set; }

    public ConnectionSettings.SourceSerializerFactory? SourceSerializerFactory { get; set; }

    public IPropertyMappingProvider? PropertyMappingProvider { get; set; }

    public ConnectionSettingsOptions()
    {
        Connection = null;
        SourceSerializerFactory = null;
        PropertyMappingProvider = null;
    }

    public ConnectionSettingsOptions UseConnection(IConnection? connection)
    {
        Connection = connection;
        return this;
    }

    public ConnectionSettingsOptions UseSourceSerializerFactory(ConnectionSettings.SourceSerializerFactory? sourceSerializerFactory)
    {
        SourceSerializerFactory = sourceSerializerFactory;
        return this;
    }

    public ConnectionSettingsOptions UsePropertyMappingProvider(IPropertyMappingProvider? propertyMappingProvider)
    {
        PropertyMappingProvider = propertyMappingProvider;
        return this;
    }
}

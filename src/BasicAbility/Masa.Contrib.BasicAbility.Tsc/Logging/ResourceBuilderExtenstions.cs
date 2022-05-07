// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace OpenTelemetry.Resources;

public static class ResourceBuilderExtenstions
{
    public static ResourceBuilder AddMasaService(
       this ResourceBuilder resourceBuilder,
       MasaLoggingOptions options)
    {
        ArgumentNullException.ThrowIfNull(resourceBuilder, nameof(resourceBuilder));

        resourceBuilder = resourceBuilder.AddService(options.ServiceName, options.ServiceNameSpace, options.ServerVersion, true, options.ServiceInstanceId);

        if (!string.IsNullOrEmpty(options.ProjectName))
        {
            resourceBuilder.AddAttributes(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>(OpenTelemetryAttributeName.Service.ProjectName, options.ProjectName) });
        }
        return resourceBuilder;
    }
}

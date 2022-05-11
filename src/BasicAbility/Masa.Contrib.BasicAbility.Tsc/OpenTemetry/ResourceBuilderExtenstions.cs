// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace OpenTelemetry.Resources;

public static class ResourceBuilderExtenstions
{
    public static ResourceBuilder AddMasaService(
       this ResourceBuilder resourceBuilder,
       MasaObservableOptions options, Action<ResourceBuilder>? action = null)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        resourceBuilder = resourceBuilder.AddService(options.ServiceName, options.ServiceNameSpace, options.ServiceVersion, true, options.ServiceInstanceId);

        var dic = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(options.ProjectName))
            dic.Add(OpenTelemetryAttributeName.Service.PROJECT_NAME, options.ProjectName);

        if (dic.Count > 0)
            resourceBuilder.AddAttributes(dic);

        resourceBuilder.AddTelemetrySdk();

        if (action != null)
            action.Invoke(resourceBuilder);

        return resourceBuilder;
    }
}

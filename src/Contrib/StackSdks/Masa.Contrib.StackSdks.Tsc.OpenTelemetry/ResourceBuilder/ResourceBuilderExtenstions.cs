// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.StackSdks.Tsc;
using Masa.Contrib.StackSdks.Tsc.OpenTelemetry;
using System;
using System.Collections.Generic;

namespace OpenTelemetry.Resources
{
    public static class ResourceBuilderExtenstions
    {
        public static ResourceBuilder AddMasaService(
           this ResourceBuilder resourceBuilder,
           MasaObservableOptions options, Action<ResourceBuilder>? action = null)
        {
            ExceptionHelper.ThrowIfNull(options);

            resourceBuilder = resourceBuilder.AddService(options.ServiceName, options.ServiceNameSpace, options.ServiceVersion, true, options.ServiceInstanceId);

            if (string.IsNullOrEmpty(options.Layer))
                options.Layer = "General";

            var dic = new Dictionary<string, object>() { { OpenTelemetryAttributeName.Service.LAYER, options.Layer } };
            if (!string.IsNullOrEmpty(options.ProjectName))
                dic.Add(OpenTelemetryAttributeName.Service.PROJECT_NAME, options.ProjectName);

            resourceBuilder.AddAttributes(dic);
            resourceBuilder.AddTelemetrySdk();

            if (action != null)
                action.Invoke(resourceBuilder);

            return resourceBuilder;
        }
    }
}

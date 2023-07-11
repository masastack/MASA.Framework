// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using OpenTelemetry.Resources;

namespace Masa.Contrib.StackSdks.Tsc.OpenTelemetry.Tests
{
    internal static class ObservableHelper
    {
        public static ResourceBuilder CreateResourceBuilder()
        {
            var options = new MasaObservableOptions
            {
                ServiceName = "test-app",
                ServiceVersion = "0.1",
                ServiceNameSpace = "develop"
            };
            return ResourceBuilder.CreateDefault().AddMasaService(options);
        }

        public const string OTLPURL = "http://localhost:4717";
    }
}

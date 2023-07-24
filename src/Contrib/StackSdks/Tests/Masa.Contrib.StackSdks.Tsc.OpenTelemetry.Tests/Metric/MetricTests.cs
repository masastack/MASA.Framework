// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.OpenTelemetry.Tests.Metric;

[TestClass]
public class MetricTests
{
    [TestMethod]
    public void AddMasaOpenTelemetryTest()
    {
        bool isConfigureCalled = false;
        IServiceCollection services = new ServiceCollection();
        services.AddMasaMetrics(builder =>
        {
            var resources = ObservableHelper.CreateResourceBuilder();
            builder.SetResourceBuilder(resources);
            builder.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(ObservableHelper.OTLPURL);
            });
            isConfigureCalled = true;
        });
        Assert.IsTrue(isConfigureCalled);
    }
}

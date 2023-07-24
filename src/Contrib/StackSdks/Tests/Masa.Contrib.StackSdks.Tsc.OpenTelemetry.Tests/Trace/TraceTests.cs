// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.OpenTelemetry.Tests.Trace;

[TestClass]
public class TraceTests
{
    [TestMethod]
    public void MasaTraceTest()
    {
        var logRecords = new List<LogRecord>();
        bool isConfigureCalled = false;
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            var resources = ObservableHelper.CreateResourceBuilder();
            var uri = new Uri(ObservableHelper.OTLPURL);

            builder.AddMasaOpenTelemetry(builder =>
            {
                builder.SetResourceBuilder(resources);
                builder.AddOtlpExporter(options =>
                {
                    if (uri != null)
                        options.Endpoint = uri;
                });
                builder.AddInMemoryExporter(logRecords);
                isConfigureCalled = true;
            });
        });

        Assert.IsTrue(isConfigureCalled);
    }
}

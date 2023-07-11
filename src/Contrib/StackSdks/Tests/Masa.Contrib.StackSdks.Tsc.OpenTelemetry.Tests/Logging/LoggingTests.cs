// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using System;
using System.Collections.Generic;

namespace Masa.Contrib.StackSdks.Tsc.OpenTelemetry.Tests.Logging
{
    [TestClass]
    public class LoggingTests
    {
        [TestMethod]
        public void AddMasaOpenTelemetryTest()
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
#if NETCOREAPP3_1
                        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
#endif
                        if (uri != null)
                            options.Endpoint = uri;
                    });
                    builder.AddInMemoryExporter(logRecords);
                    isConfigureCalled = true;
                });
            });

            Assert.IsTrue(isConfigureCalled);
            var logger = loggerFactory.CreateLogger("OtlpLogExporterTests");
            logger.LogInformation("Hello from {name} {price}.", "tomato", 2.99);
            Assert.IsTrue(logRecords.Count == 1);
            var logRecord = logRecords[0];
            Assert.IsNotNull(logRecord.Attributes);
        }
    }
}

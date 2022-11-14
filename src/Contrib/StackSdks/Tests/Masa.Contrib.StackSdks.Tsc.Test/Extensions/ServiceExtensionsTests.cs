// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq.Protected;
using OpenTelemetry.Logs;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Masa.Contrib.StackSdks.Tsc.Tests.Extensions;

[TestClass]
public class ServiceExtensionsTests
{
    [TestMethod]
    public void ConfigrationTest()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.Services.AddObservable(builder, configuration);
        });
        Assert.IsNotNull(configuration.GetSection("Masa:Observable").Get<MasaObservableOptions>());
    }

    [TestMethod]
    public void FuncTest()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.Services.AddObservable(builder, () => new MasaObservableOptions
            {
                ServiceName = "test-app"
            }, () => ObservableHelper.OTLPURL, true);
        });
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void OptionsTest()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.Services.AddObservable(builder, new MasaObservableOptions
            {
                ServiceName = "test-app"
            }, ObservableHelper.OTLPURL, true);
        });
        Assert.IsTrue(true);
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Clickhouse.Tests.Extensions;

[TestClass]
public class ServiceExtensitionsTests
{
    [TestMethod]
    public void AddMASAStackClickhouseTest()
    {
        var service = new ServiceCollection();
        service.AddMASAStackClickhouse(Consts.ConnectionString);
        var logService = service.BuildServiceProvider().GetRequiredService<ILogService>();
        Assert.IsNotNull(logService);
    }

    [TestMethod]
    public void AddMASAStackClickhouseTestCustomTable()
    {
        var service = new ServiceCollection();
        service.AddMASAStackClickhouse(Consts.ConnectionString,"otel_log","custom_log","otel_trace","custom_trace");
        var logService = service.BuildServiceProvider().GetRequiredService<ILogService>();
        Assert.IsNotNull(logService);
    }
}

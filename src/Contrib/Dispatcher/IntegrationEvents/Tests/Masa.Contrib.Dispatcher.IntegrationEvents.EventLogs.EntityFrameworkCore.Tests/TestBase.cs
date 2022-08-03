// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EntityFrameworkCore.Tests;

public class TestBase : IDisposable
{
    protected readonly string ConnectionString = "DataSource=:memory:";
    protected readonly SqliteConnection Connection;

    protected TestBase()
    {
        Connection = new SqliteConnection(ConnectionString);
        Connection.Open();
    }

    public void Dispose()
    {
        Connection.Close();
    }

    protected IDispatcherOptions CreateDispatcherOptions(IServiceCollection services, Assembly[]? assemblies = null)
    {
        Mock<IDispatcherOptions> dispatcherOptions = new();
        dispatcherOptions.Setup(option => option.Services).Returns(services).Verifiable();
        dispatcherOptions.Setup(option => option.Assemblies).Returns(assemblies ?? AppDomain.CurrentDomain.GetAssemblies())
            .Verifiable();
        return dispatcherOptions.Object;
    }
}

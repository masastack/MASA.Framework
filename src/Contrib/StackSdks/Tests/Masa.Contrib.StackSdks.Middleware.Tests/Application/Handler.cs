// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Middleware.Tests.Application;

public class Handler
{
#pragma warning disable CA1822
    [EventHandler]
    public void TestCommand(TestCommand testCommand)
    {

    }

    [EventHandler]
    public void TestAllowCommand(TestAllowCommand testCommand)
    {
        testCommand.Count++;
    }

    [EventHandler]
    public void TestQuery(TestQuery testQuery)
    {
        testQuery.Result = true;
    }
#pragma warning restore CA1822
}

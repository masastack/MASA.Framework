// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Dispatcher.Events;
using Masa.Contrib.Dispatcher.Events;

namespace WebApplication1.ApplicationServices.RegisterUser;

public record RegisterUserEvent : Event
{
    public string Name { get; set; }
}

public class RegisterUserEventHandler
{
    [EventHandler]
    public void Test(RegisterUserEvent @event,ILogger<RegisterUserEventHandler> logger)
    {
        logger.LogInformation("注册用户事件");
    }
}

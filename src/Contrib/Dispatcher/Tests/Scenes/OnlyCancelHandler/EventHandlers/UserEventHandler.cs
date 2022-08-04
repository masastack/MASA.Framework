// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Scenes.OnlyCancelHandler.EventHandlers;

public class UserEventHandler
{
    [EventHandler(IsCancel = true)]
    public void BindPhoneNumber(BindPhoneNumberEvent @event)
    {

    }
}

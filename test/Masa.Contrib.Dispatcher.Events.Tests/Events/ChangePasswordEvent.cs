// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Events;

public class ChangePasswordEvent : IEvent
{
    public string Account { get; set; }

    public string Content { get; set; }

    public Guid Id => Guid.NewGuid();

    public DateTime CreationTime => DateTime.UtcNow;
}

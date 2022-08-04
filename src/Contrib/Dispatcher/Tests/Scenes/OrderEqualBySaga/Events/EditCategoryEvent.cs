// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Scenes.OrderEqualBySaga.Events;

public record EditCategoryEvent : Event
{
    public string CategoryId { get; set; }

    public string CategoryName { get; set; }
}

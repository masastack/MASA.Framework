// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events.Tests.Scenes.OrderEqualBySaga;

public record EditCategoryEvent : Event
{
    public string CategoryId { get; set; }

    public string CategoryName { get; set; }
}

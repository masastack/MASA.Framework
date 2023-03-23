﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Dispatcher.Events.Tests.Scenes.IntegrationEvent")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

internal interface ILocalEventBus : IEventBus
{

}

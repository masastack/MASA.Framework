// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Dispatcher.IntegrationEvents")]
[assembly: InternalsVisibleTo("Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests")]
[assembly: InternalsVisibleTo("Masa.Contrib.Dispatcher.IntegrationEvents.Tests")]
[assembly: InternalsVisibleTo("Masa.Contrib.Isolation")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

internal interface ILocalMessageDbConnectionStringProvider
{
    List<MasaDbContextConfigurationOptions> DbContextOptionsList { get; }
}

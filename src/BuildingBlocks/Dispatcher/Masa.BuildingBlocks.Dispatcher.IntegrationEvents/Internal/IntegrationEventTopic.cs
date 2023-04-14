// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

/// <summary>
/// Only used to get Topic
/// Avoid directly obtaining the value of the Topic because you are worried that the global JsonSerializerOptions will be modified, resulting in failure to obtain the Topic
/// </summary>
internal class IntegrationEventTopic : ITopic
{
    public string Topic { get; set; }
}

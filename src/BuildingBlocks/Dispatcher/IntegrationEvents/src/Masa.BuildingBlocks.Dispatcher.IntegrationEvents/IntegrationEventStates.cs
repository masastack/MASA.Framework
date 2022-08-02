// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

public enum IntegrationEventStates
{
    NotPublished = 0,
    InProgress = 1,
    Published = 2,
    PublishedFailed = 3
}

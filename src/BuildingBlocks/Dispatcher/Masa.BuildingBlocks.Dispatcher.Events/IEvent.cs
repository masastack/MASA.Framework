// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Dispatcher.Events;

public interface IEvent
{
    Guid GetEventId();

    void SetEventId(Guid eventId);

    DateTime GetCreationTime();

    void SetCreationTime(DateTime creationTime);
}

public interface IEvent<TResult> : IEvent
{
    TResult Result { get; set; }
}

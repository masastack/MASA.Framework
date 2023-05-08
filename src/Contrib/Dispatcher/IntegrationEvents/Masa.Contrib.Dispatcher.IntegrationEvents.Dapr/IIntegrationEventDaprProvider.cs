// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

public interface IIntegrationEventDaprProvider
{
    /// <summary>
    /// Get the dapr appid used by integration events
    /// priority: User specified appid > environment variable > global appId
    /// </summary>
    /// <returns></returns>
    string? GetDaprAppId(string? appId);
}

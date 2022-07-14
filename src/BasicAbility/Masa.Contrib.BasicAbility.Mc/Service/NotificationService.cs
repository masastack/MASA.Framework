// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Mc.Service;

public class NotificationService : INotificationService
{
    readonly ICallerProvider _callerProvider;
    readonly string _party = "api/notification";

    public NotificationService(ICallerProvider callerProvider)
    {
        _callerProvider = callerProvider;
    }

    public async Task SendNotificationAsync(SendNotificationModel options)
    {
        var requestUri = $"{_party}/SendNotification";
        await _callerProvider.PostAsync(requestUri, options);
    }
}

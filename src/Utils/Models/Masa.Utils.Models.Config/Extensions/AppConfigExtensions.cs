// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Models.Config;

public static class AppConfigExtensions
{
    public static string? GetAppId(this AppConfig config)
    {
        return config.AppId ??(Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name;
    }
}

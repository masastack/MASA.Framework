// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Development.Dapr;

public static class DaprExtensions
{
    public static string DefaultAppId => ((Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name ??
        throw new NotSupportedException("dapr appid is not empty")).Replace(".", Const.DEFAULT_APPID_DELIMITER);

    /// <summary>
    /// Appid suffix, the default is the current MAC address
    /// </summary>
    public static readonly string DefaultAppidSuffix = NetworkUtils.GetPhysicalAddress();

    /// <summary>
    /// Get dapr AppId by appid and suffix
    /// </summary>
    /// <param name="appId">custom appId</param>
    /// <param name="appidSuffix">appid suffix, When appidSuffix is empty, Dapr appId is custom appId, When appidSuffix is null, appidSuffix is MAC address, default: null</param>
    /// <param name="appIdDelimiter">separator used to splice custom appId and appIdSuffix, default: -</param>
    /// <returns></returns>
    /// <returns></returns>
    public static string GetAppId(string appId, string? appidSuffix = null, string appIdDelimiter = Const.DEFAULT_APPID_DELIMITER)
    {
        ArgumentNullException.ThrowIfNull(appIdDelimiter, nameof(appIdDelimiter));

        if (appidSuffix == null)
            appidSuffix = DefaultAppidSuffix;
        else if (appidSuffix.Trim() == string.Empty)
            return appId;

        return GetAppIdCore(appId, appidSuffix, appIdDelimiter);
    }

    private static string GetAppIdCore(string appId, string appidSuffix, string appIdDelimiter)
    {
        if (appIdDelimiter == ".")
            throw new NotSupportedException("AppIdDelimiter is not supported as .");

        return $"{appId}{appIdDelimiter}{appidSuffix}";
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Development.DaprStarter.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Development.DaprStarter;

internal class DefaultDaprProvider : IDaprProvider
{
    private readonly IOptions<MasaAppConfigureOptions>? _masaAppConfigureOptions;
    private readonly IConfiguration? _configuration;
    private readonly IMasaConfiguration? _masaConfiguration;

    /// <summary>
    /// Use after getting dapr AppId and global AppId fails
    /// </summary>
    private static readonly string DefaultAppId = (
        Assembly.GetEntryAssembly() ??
        Assembly.GetCallingAssembly()).GetName().Name!.Replace(".", DaprStarterConstant.DEFAULT_APPID_DELIMITER);

    public DefaultDaprProvider(IOptions<MasaAppConfigureOptions>? masaAppConfigureOptions = null,
        IConfiguration? configuration = null,
        IMasaConfiguration? masaConfiguration = null)
    {
        _masaAppConfigureOptions = masaAppConfigureOptions;
        _configuration = configuration;
        _masaConfiguration = masaConfiguration;
    }

    public string CompletionAppId(string? appId = null,
        bool disableAppIdSuffix = false,
        string? appIdSuffix = null,
        string appIdDelimiter = DaprStarterConstant.DEFAULT_APPID_DELIMITER)
    {
        string? actualAppId = appId;
        if (actualAppId.IsNullOrWhiteSpace())
            actualAppId = _masaAppConfigureOptions?.Value.AppId;
        if (actualAppId.IsNullOrWhiteSpace())
            actualAppId = DefaultAppId;
        if (IsIncompleteAppId())
            actualAppId = $"{actualAppId}{appIdDelimiter}{appIdSuffix ?? NetworkUtils.GetPhysicalAddress()}";

        var value = _configuration == null ? Environment.GetEnvironmentVariable(actualAppId) : _configuration?.GetSection(actualAppId).Value;
        if (value.IsNullOrWhiteSpace())
            value = _masaConfiguration?.Local.GetSection(actualAppId).Value;

        if (value.IsNullOrWhiteSpace()) return actualAppId;

        return value;

        bool IsIncompleteAppId() => !disableAppIdSuffix && (appIdSuffix == null || appIdSuffix.Trim() != string.Empty);
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter.Tests;

[TestClass]
public class DefaultDaprProviderTest
{
    private static readonly string DefaultAppIdSuffix = NetworkUtils.GetPhysicalAddress();

    private static readonly string DefaultAppId = (
        Assembly.GetEntryAssembly() ??
        Assembly.GetCallingAssembly()).GetName().Name!.Replace(".", DaprStarterConstant.DEFAULT_APPID_DELIMITER);

    [DataRow(null, "test2", "test2-{default-suffix}", "test3", false, null, null, "test2-{default-suffix}")]
    [DataRow(null, "test2", "test2-{default-suffix}-2", "test3", false, null, null, "test2-{default-suffix}")]
    [DataRow(null, null, "{default-appid}-{default-suffix}", "test3", false, null, null, "{default-appid}-{default-suffix}")]
    [DataRow(null, null, "{default-appid}-{default-suffix}", "test3", false, null, null, "{default-appid}-{default-suffix}")]
    [DataRow(null, "test2", "test2-masa", "test3", false, "masa", null, "test2-masa")]
    [DataRow(null, "test2", "test2-masa-2", "test3", false, "masa", null, "test2-masa")]
    [DataRow(null, null, "{default-appid}-masa", "test3", false, "masa", null, "{default-appid}-masa")]
    [DataRow(null, null, "{default-appid}-masa-2", "test3", false, "masa", null, "{default-appid}-masa")]
    [DataRow(null, "test2", "test2|masa", "test3", false, "masa", "|", "test2|masa")]
    [DataRow(null, "test2", "test2|masa-2", "test3", false, "masa", "|", "test2|masa")]
    [DataRow(null, null, "{default-appid}|masa", "test3", false, "masa", "|", "{default-appid}|masa")]
    [DataRow(null, null, "{default-appid}|masa-2", "test3", false, "masa", "|", "{default-appid}|masa")]
    [DataRow(null, "test2", "test2|{default-suffix}", "test3", false, null, "|", "test2|{default-suffix}")]
    [DataRow(null, "test2", "test2|{default-suffix}-2", "test3", false, null, "|", "test2|{default-suffix}")]
    [DataRow(null, null, "{default-appid}|{default-suffix}", "test3", false, null, "|", "{default-appid}|{default-suffix}")]
    [DataRow(null, null, "{default-appid}|{default-suffix}-2", "test3", false, null, "|", "{default-appid}|{default-suffix}")]
    [DataRow("test", "test2", "test-{default-suffix}", "test3", false, null, null, "test-{default-suffix}")]
    [DataRow("test", "test2", "test-{default-suffix}-2", "test3", false, null, null, "test-{default-suffix}")]
    [DataRow("test", null, "test-{default-suffix}", "test3", false, null, null, "test-{default-suffix}")]
    [DataRow("test", null, "test-{default-suffix}-2", "test3", false, null, null, "test-{default-suffix}")]
    [DataRow("test", "test2", "test-masa", "test3", false, "masa", null, "test-masa")]
    [DataRow("test", "test2", "test-masa-2", "test3", false, "masa", null, "test-masa")]
    [DataRow("test", null, "test-masa", "test3", false, "masa", null, "test-masa")]
    [DataRow("test", null, "test-masa-2", "test3", false, "masa", null, "test-masa")]
    [DataRow("test", "test2", "test|masa", "test3", false, "masa", "|", "test|masa")]
    [DataRow("test", "test2", "test|masa-2", "test3", false, "masa", "|", "test|masa")]
    [DataRow("test", null, "test|masa", "test3", false, "masa", "|", "test|masa")]
    [DataRow("test", null, "test|masa-2", "test3", false, "masa", "|", "test|masa")]
    [DataRow("test", "test2", "test|{default-suffix}", "test3", false, null, "|", "test|{default-suffix}")]
    [DataRow("test", "test2", "test|{default-suffix}-2", "test3", false, null, "|", "test|{default-suffix}")]
    [DataRow("test", null, "test|{default-suffix}", "test3", false, null, "|", "test|{default-suffix}")]
    [DataRow("test", null, "test|{default-suffix}-2", "test3", false, null, "|", "test|{default-suffix}")]
    [DataRow("test", "test2", "test", "test3", true, null, null, "test")]
    [DataRow("test", "test2", "test-2", "test3", true, null, null, "test")]
    [DataRow("test", null, "test", "test3", true, null, null, "test")]
    [DataRow("test", null, "test-2", "test3", true, null, null, "test")]
    [DataRow("test", "test2", "test", "test3", true, "masa", null, "test")]
    [DataRow("test", "test2", "test-2", "test3", true, "masa", null, "test")]
    [DataRow("test", null, "test", "test3", true, "masa", null, "test")]
    [DataRow("test", null, "test-2", "test3", true, "masa", null, "test")]
    [DataRow("test", "test2", "test", "test3", true, "masa", "|", "test")]
    [DataRow("test", "test2", "test-2", "test3", true, "masa", "|", "test")]
    [DataRow("test", null, "test", "test3", true, "masa", "|", "test")]
    [DataRow("test", null, "test-2", "test3", true, "masa", "|", "test")]
    [DataRow("test", "test2", "test", "test3", true, null, "|", "test")]
    [DataRow("test", "test2", "test-2", "test3", true, null, "|", "test")]
    [DataRow("test", null, "test", "test3", true, null, "|", "test")]
    [DataRow("test", null, "test-2", "test3", true, null, "|", "test")]
    [DataTestMethod]
    public void TestCompletionAppId(
        string? inputAppId,
        string? inputGlobalAppId,
        string? environmentAppIdKey,
        string? environmentAppIdValue,
        bool inputDisableAppIdSuffix,
        string? inputAppIdSuffix,
        string? inputAppIdDelimiter,
        string expectedAppId)
    {
        if (environmentAppIdKey != null && environmentAppIdValue != null)
        {
            Environment.SetEnvironmentVariable(
                environmentAppIdKey.Replace("{default-appid}", DefaultAppId).Replace("{default-suffix}", DefaultAppIdSuffix),
                environmentAppIdValue);
        }

        IOptions<MasaAppConfigureOptions>? masaAppConfigureOptions = inputGlobalAppId == null
            ? null
            : Options.Create(new MasaAppConfigureOptions()
            {
                AppId = inputGlobalAppId
            });
        var defaultDaprProvider = new DefaultDaprProvider(masaAppConfigureOptions);

        var acceptAppId = inputAppIdDelimiter != null
            ? defaultDaprProvider.CompletionAppId(inputAppId, inputDisableAppIdSuffix, inputAppIdSuffix, inputAppIdDelimiter)
            : defaultDaprProvider.CompletionAppId(inputAppId, inputDisableAppIdSuffix, inputAppIdSuffix);


        if (environmentAppIdKey != null && !environmentAppIdValue.IsNullOrWhiteSpace())
        {
            Assert.AreEqual(
                environmentAppIdKey.Replace("{default-appid}", DefaultAppId).Replace("{default-suffix}", DefaultAppIdSuffix) ==
                expectedAppId.Replace("{default-appid}", DefaultAppId).Replace("{default-suffix}", DefaultAppIdSuffix)
                    ? environmentAppIdValue
                    : expectedAppId.Replace("{default-appid}", DefaultAppId).Replace("{default-suffix}", DefaultAppIdSuffix), acceptAppId);
        }
        else
        {
            Assert.AreEqual(
                expectedAppId.Replace("{default-appid}", DefaultAppId).Replace("{default-suffix}", DefaultAppIdSuffix), acceptAppId);
        }

        if (environmentAppIdKey != null && environmentAppIdValue != null)
        {
            Environment.SetEnvironmentVariable(
                environmentAppIdKey.Replace("{default-appid}", DefaultAppId).Replace("{default-suffix}", DefaultAppIdSuffix),
                "");
        }
    }

    [DataRow(null, "test2", false, null, null, "test2-{default-suffix}")]
    [DataRow(null, null, false, null, null, "{default-appid}-{default-suffix}")]
    [DataRow(null, "test2", false, "masa", null, "test2-masa")]
    [DataRow(null, null, false, "masa", null, "{default-appid}-masa")]
    [DataRow(null, "test2", false, "masa", "|", "test2|masa")]
    [DataRow(null, null, false, "masa", "|", "{default-appid}|masa")]
    [DataRow(null, "test2", false, null, "|", "test2|{default-suffix}")]
    [DataRow(null, null, false, null, "|", "{default-appid}|{default-suffix}")]
    [DataRow("test", "test2", false, null, null, "test-{default-suffix}")]
    [DataRow("test", null, false, null, null, "test-{default-suffix}")]
    [DataRow("test", "test2", false, "masa", null, "test-masa")]
    [DataRow("test", null, false, "masa", null, "test-masa")]
    [DataRow("test", "test2", false, "masa", "|", "test|masa")]
    [DataRow("test", null, false, "masa", "|", "test|masa")]
    [DataRow("test", "test2", false, null, "|", "test|{default-suffix}")]
    [DataRow("test", null, false, null, "|", "test|{default-suffix}")]
    [DataRow("test", "test2", true, null, null, "test")]
    [DataRow("test", null, true, null, null, "test")]
    [DataRow("test", "test2", true, "masa", null, "test")]
    [DataRow("test", null, true, "masa", null, "test")]
    [DataRow("test", "test2", true, "masa", "|", "test")]
    [DataRow("test", null, true, "masa", "|", "test")]
    [DataRow("test", "test2", true, null, "|", "test")]
    [DataRow("test", null, true, null, "|", "test")]
    [DataTestMethod]
    public void TestCompletionAppIdByConfiguration(
        string? inputAppId,
        string? inputGlobalAppId,
        bool inputDisableAppIdSuffix,
        string? inputAppIdSuffix,
        string? inputAppIdDelimiter,
        string expectedAppId)
    {
        IOptions<MasaAppConfigureOptions>? masaAppConfigureOptions = inputGlobalAppId == null
            ? null
            : Options.Create(new MasaAppConfigureOptions()
            {
                AppId = inputGlobalAppId
            });
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var defaultDaprProvider = new DefaultDaprProvider(masaAppConfigureOptions, configuration);

        var acceptAppId = inputAppIdDelimiter != null
            ? defaultDaprProvider.CompletionAppId(inputAppId, inputDisableAppIdSuffix, inputAppIdSuffix, inputAppIdDelimiter)
            : defaultDaprProvider.CompletionAppId(inputAppId, inputDisableAppIdSuffix, inputAppIdSuffix);


        Assert.AreEqual(
            expectedAppId == "test-masa"
                ? "test3"
                : expectedAppId.Replace("{default-appid}", DefaultAppId).Replace("{default-suffix}", DefaultAppIdSuffix), acceptAppId);
    }

    [DataRow(null, "test2", false, null, null, "test2-{default-suffix}")]
    [DataRow(null, null, false, null, null, "{default-appid}-{default-suffix}")]
    [DataRow(null, "test2", false, "masa", null, "test2-masa")]
    [DataRow(null, null, false, "masa", null, "{default-appid}-masa")]
    [DataRow(null, "test2", false, "masa", "|", "test2|masa")]
    [DataRow(null, null, false, "masa", "|", "{default-appid}|masa")]
    [DataRow(null, "test2", false, null, "|", "test2|{default-suffix}")]
    [DataRow(null, null, false, null, "|", "{default-appid}|{default-suffix}")]
    [DataRow("test", "test2", false, null, null, "test-{default-suffix}")]
    [DataRow("test", null, false, null, null, "test-{default-suffix}")]
    [DataRow("test", "test2", false, "masa", null, "test-masa")]
    [DataRow("test", null, false, "masa", null, "test-masa")]
    [DataRow("test", "test2", false, "masa", "|", "test|masa")]
    [DataRow("test", null, false, "masa", "|", "test|masa")]
    [DataRow("test", "test2", false, null, "|", "test|{default-suffix}")]
    [DataRow("test", null, false, null, "|", "test|{default-suffix}")]
    [DataRow("test", "test2", true, null, null, "test")]
    [DataRow("test", null, true, null, null, "test")]
    [DataRow("test", "test2", true, "masa", null, "test")]
    [DataRow("test", null, true, "masa", null, "test")]
    [DataRow("test", "test2", true, "masa", "|", "test")]
    [DataRow("test", null, true, "masa", "|", "test")]
    [DataRow("test", "test2", true, null, "|", "test")]
    [DataRow("test", null, true, null, "|", "test")]
    [DataTestMethod]
    public void TestCompletionAppIdByMasaConfiguration(
        string? inputAppId,
        string? inputGlobalAppId,
        bool inputDisableAppIdSuffix,
        string? inputAppIdSuffix,
        string? inputAppIdDelimiter,
        string expectedAppId)
    {
        IOptions<MasaAppConfigureOptions>? masaAppConfigureOptions = inputGlobalAppId == null
            ? null
            : Options.Create(new MasaAppConfigureOptions()
            {
                AppId = inputGlobalAppId
            });
        var services = new ServiceCollection();
        services.AddMasaConfiguration(optionsBuilder =>
        {
        });
        var serviceProvider = services.BuildServiceProvider();
        var defaultDaprProvider = new DefaultDaprProvider(
            masaAppConfigureOptions,
            serviceProvider.GetService<IConfiguration>(),
            serviceProvider.GetService<IMasaConfiguration>());

        var acceptAppId = inputAppIdDelimiter != null
            ? defaultDaprProvider.CompletionAppId(inputAppId, inputDisableAppIdSuffix, inputAppIdSuffix, inputAppIdDelimiter)
            : defaultDaprProvider.CompletionAppId(inputAppId, inputDisableAppIdSuffix, inputAppIdSuffix);


        Assert.AreEqual(
            expectedAppId == "test-masa"
                ? "test3"
                : expectedAppId.Replace("{default-appid}", DefaultAppId).Replace("{default-suffix}", DefaultAppIdSuffix), acceptAppId);
    }
}

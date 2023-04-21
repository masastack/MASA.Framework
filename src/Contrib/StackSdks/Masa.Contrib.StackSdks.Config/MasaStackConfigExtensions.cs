// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Config;

public static class MasaStackConfigExtensions
{
    internal static JsonArray GetMasaStack(this IMasaStackConfig masaStackConfig)
    {
        var value = masaStackConfig.GetValue(MasaStackConfigConstant.MASA_STACK);
        if (string.IsNullOrEmpty(value))
        {
            return new();
        }
        return JsonSerializer.Deserialize<JsonArray>(value) ?? new();
    }

    public static List<JsonNode> GetAllService(this IMasaStackConfig masaStackConfig)
    {
        var webs = GetMasaStack(masaStackConfig).Select(jsonObject => jsonObject?[MasaStackConstant.SERVICE]!).ToList();
        webs.RemoveAll(i => i == null);
        return webs ?? new();
    }

    public static List<JsonNode> GetAllWeb(this IMasaStackConfig masaStackConfig)
    {
        var webs = GetMasaStack(masaStackConfig).Select(jsonObject => jsonObject?[MasaStackConstant.WEB]!).ToList();
        webs.RemoveAll(i => i == null);
        return webs ?? new();
    }

    public static bool HasAlert(this IMasaStackConfig masaStackConfig)
    {
        return GetMasaStack(masaStackConfig).Any(jsonObject => jsonObject?["id"]?.ToString() == MasaStackConstant.ALERT);
    }

    public static bool HasTsc(this IMasaStackConfig masaStackConfig)
    {
        return GetMasaStack(masaStackConfig).Any(jsonObject => jsonObject?["id"]?.ToString() == MasaStackConstant.TSC);
    }

    public static bool HasScheduler(this IMasaStackConfig masaStackConfig)
    {
        return GetMasaStack(masaStackConfig).Any(jsonObject => jsonObject?["id"]?.ToString() == MasaStackConstant.SCHEDULER);
    }

    public static string GetConnectionString(this IMasaStackConfig masaStackConfig, string projectName)
    {
        var connStr = masaStackConfig.GetValue(MasaStackConfigConstant.CONNECTIONSTRING);
        var dbModel = JsonSerializer.Deserialize<DbModel>(connStr);
        var databaseName = $"{projectName}{(string.IsNullOrWhiteSpace(masaStackConfig.SuffixIdentity) ? "" : "_" + masaStackConfig.SuffixIdentity)}";

        return dbModel?.ToString(databaseName) ?? "";
    }

    public static string GetHost(this IMasaStackConfig masaStackConfig, string project, string app)
    {
        return GetMasaStack(masaStackConfig).FirstOrDefault(i => i?["id"]?.ToString() == project)?[app]?["host"]?.ToString() ?? "";
    }


    public static string GetAuthServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetHost(masaStackConfig, MasaStackConstant.AUTH, MasaStackConstant.SERVICE);
    }

    public static string GetPmServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetHost(masaStackConfig, MasaStackConstant.PM, MasaStackConstant.SERVICE);
    }

    public static string GetDccServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return MasaStackConfigUtils.GetDccServiceDomain(masaStackConfig.GetValues());
    }

    public static string GetTscServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetHost(masaStackConfig, MasaStackConstant.TSC, MasaStackConstant.SERVICE);
    }

    public static string GetAlertServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetHost(masaStackConfig, MasaStackConstant.ALERT, MasaStackConstant.SERVICE);
    }

    public static string GetMcServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetHost(masaStackConfig, MasaStackConstant.MC, MasaStackConstant.SERVICE);
    }

    public static string GetSchedulerServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetHost(masaStackConfig, MasaStackConstant.SCHEDULER, MasaStackConstant.SERVICE);
    }

    public static string GetSchedulerWorkerDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetHost(masaStackConfig, MasaStackConstant.SCHEDULER, MasaStackConstant.WORKER);
    }

    public static string GetSsoDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetHost(masaStackConfig, MasaStackConstant.AUTH, MasaStackConstant.SSO);
    }

    public static IEnumerable<(string, string, string)> GetAllUINames(this IMasaStackConfig masaStackConfig)
    {
        foreach (var web in GetAllWeb(masaStackConfig))
        {
            var id = web["id"]?.ToString() ?? "";
            var name = web["name"]?.ToString() ?? "";
            var host = web["host"]?.ToString() ?? "";
            if (string.IsNullOrEmpty(id))
            {
                continue;
            }
            yield return (id, name, host);
        }
    }

    public static string GetServiceId(this IMasaStackConfig masaStackConfig, string project)
    {
        return masaStackConfig.GetMasaStack().FirstOrDefault(i => i?["id"]?.ToString() == project)
            ?[MasaStackConstant.SERVICE]?["id"]?.ToString() ?? "";
    }

    public static string GetWebId(this IMasaStackConfig masaStackConfig, string project)
    {
        return masaStackConfig.GetMasaStack().FirstOrDefault(i => i?["id"]?.ToString() == project)
            ?[MasaStackConstant.WEB]?["id"]?.ToString() ?? "";
    }
}

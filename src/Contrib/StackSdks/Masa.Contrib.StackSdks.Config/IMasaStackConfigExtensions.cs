// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Text.Json.Nodes;

namespace Masa.Contrib.StackSdks.Config;

public static class IMasaStackConfigExtensions
{
    public static Dictionary<string, JsonObject> GetAllServer(this IMasaStackConfig masaStackConfig)
    {
        return JsonSerializer.Deserialize<Dictionary<string, JsonObject>>(masaStackConfig.GetValue(MasaStackConfigConst.MASA_ALL_SERVER)) ?? new();
    }

    public static bool HasAlert(this IMasaStackConfig masaStackConfig)
    {
        return GetAllServer(masaStackConfig).ContainsKey("alert");
    }

    public static bool HasTsc(this IMasaStackConfig masaStackConfig)
    {
        return GetAllServer(masaStackConfig).ContainsKey("tsc");
    }

    public static bool HasScheduler(this IMasaStackConfig masaStackConfig)
    {
        return GetAllServer(masaStackConfig).ContainsKey("scheduler");
    }

    public static string GetConnectionString(this IMasaStackConfig masaStackConfig, string datebaseName)
    {
        var connStr = masaStackConfig.GetValue(MasaStackConfigConst.CONNECTIONSTRING);
        var dbModel = JsonSerializer.Deserialize<DbModel>(connStr);

        return dbModel?.ToString(datebaseName) ?? "";
    }

    public static string GetDomain(this IMasaStackConfig masaStackConfig, string project, string service)
    {
        var domain = "";
        GetAllServer(masaStackConfig).TryGetValue(project, out JsonObject? jsonObject);
        if (jsonObject != null)
        {
            var secondaryDomain = jsonObject[service]?.ToString();
            if (secondaryDomain != null)
            {
                domain = $"{secondaryDomain}.{masaStackConfig.GetValue(MasaStackConfigConst.DOMAIN_NAME).TrimStart('.')}";
            }
        }
        return domain;
    }

    public static string GetAuthServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, "auth", "server");
    }

    public static string GetPmServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, "pm", "server");
    }

    public static string GetDccServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, "dcc", "server");
    }

    public static string GetTscServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, "tsc", "server");
    }

    public static string GetAlertServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, "alert", "server");
    }

    public static string GetMcServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, "mc", "server");
    }

    public static string GetSchedulerServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, "scheduler", "server");
    }

    public static string GetWorkerDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, "scheduler", "worker");
    }

    public static string GetSsoDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, "auth", "sso");
    }

    public static IEnumerable<string> GetAllUINames(this IMasaStackConfig masaStackConfig)
    {
        foreach (var server in GetAllServer(masaStackConfig))
        {
            var uiName = server.Value["ui"]?.ToString();
            if (string.IsNullOrEmpty(uiName))
            {
                continue;
            }
            yield return uiName;
        }
    }
}

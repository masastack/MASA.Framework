// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Config;

public static class MasaStackConfigExtensions
{
    public static JsonArray GetMasaStack(this IMasaStackConfig masaStackConfig)
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
        var webs = GetMasaStack(masaStackConfig).Select(jsonObject => jsonObject?[MasaStackApp.Service.Name]!).ToList();
        webs.RemoveAll(i => i == null);
        return webs ?? new();
    }

    public static List<JsonNode> GetAllWeb(this IMasaStackConfig masaStackConfig)
    {
        var webs = GetMasaStack(masaStackConfig).Select(jsonObject => jsonObject?[MasaStackApp.WEB.Name]!).ToList();
        webs.RemoveAll(i => i == null);
        return webs ?? new();
    }

    public static bool HasAlert(this IMasaStackConfig masaStackConfig)
    {
        return GetMasaStack(masaStackConfig).Any(jsonObject => jsonObject?["id"]?.ToString() == MasaStackProject.Alert.Name);
    }

    public static bool HasTsc(this IMasaStackConfig masaStackConfig)
    {
        return GetMasaStack(masaStackConfig).Any(jsonObject => jsonObject?["id"]?.ToString() == MasaStackProject.TSC.Name);
    }

    public static bool HasScheduler(this IMasaStackConfig masaStackConfig)
    {
        return GetMasaStack(masaStackConfig).Any(jsonObject => jsonObject?["id"]?.ToString() == MasaStackProject.Scheduler.Name);
    }

    public static string GetConnectionString(this IMasaStackConfig masaStackConfig, string projectName)
    {
        var connStr = masaStackConfig.GetValue(MasaStackConfigConstant.CONNECTIONSTRING);
        var dbModel = JsonSerializer.Deserialize<DbModel>(connStr);
        var databaseName = $"{projectName}{(string.IsNullOrWhiteSpace(masaStackConfig.SuffixIdentity) ? "" : "_" + masaStackConfig.SuffixIdentity)}";

        return dbModel?.ToString(databaseName) ?? "";
    }

    public static string GetDomain(this IMasaStackConfig masaStackConfig, MasaStackProject project, MasaStackApp app)
    {
        return GetMasaStack(masaStackConfig).FirstOrDefault(i => i?["id"]?.ToString() == project.Name)?[app.Name]?["domain"]?.ToString() ?? "";
    }


    public static string GetAuthServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, MasaStackProject.Auth, MasaStackApp.Service);
    }

    public static string GetPmServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, MasaStackProject.PM, MasaStackApp.Service);
    }

    public static string GetDccServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, MasaStackProject.DCC, MasaStackApp.Service);
    }

    public static string GetTscServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, MasaStackProject.TSC, MasaStackApp.Service);
    }

    public static string GetAlertServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, MasaStackProject.Alert, MasaStackApp.Service);
    }

    public static string GetMcServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, MasaStackProject.MC, MasaStackApp.Service);
    }

    public static string GetSchedulerServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, MasaStackProject.Scheduler, MasaStackApp.Service);
    }

    public static string GetSchedulerWorkerDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, MasaStackProject.Scheduler, MasaStackApp.Worker);
    }

    public static string GetSsoDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetDomain(masaStackConfig, MasaStackProject.Auth, MasaStackApp.SSO);
    }

    public static IEnumerable<KeyValuePair<string, string>> GetUIDomainPairs(this IMasaStackConfig masaStackConfig)
    {
        foreach (var web in GetAllWeb(masaStackConfig))
        {
            var id = web["id"]?.ToString() ?? "";
            var domain = web["domain"]?.ToString() ?? "";
            if (string.IsNullOrEmpty(id))
            {
                continue;
            }
            yield return new KeyValuePair<string, string>(id, domain);
        }
    }

    public static string GetId(this IMasaStackConfig masaStackConfig, MasaStackProject project, MasaStackApp app)
    {
        return masaStackConfig.GetMasaStack().FirstOrDefault(i => i?["id"]?.ToString() == project.Name)
            ?[app.Name]?["id"]?.ToString() ?? "";
    }

    public static string GetServiceId(this IMasaStackConfig masaStackConfig, MasaStackProject project)
    {
        return masaStackConfig.GetId(project, MasaStackApp.Service);
    }

    public static string GetWebId(this IMasaStackConfig masaStackConfig, MasaStackProject project)
    {
        return masaStackConfig.GetId(project, MasaStackApp.WEB);
    }
}

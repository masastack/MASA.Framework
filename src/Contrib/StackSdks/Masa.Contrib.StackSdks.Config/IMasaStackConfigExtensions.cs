// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Config;

public static class IMasaStackConfigExtensions
{
    public static Dictionary<string, JsonObject> GetAllServer(this IMasaStackConfig masaStackConfig)
    {
        return JsonSerializer.Deserialize<Dictionary<string, JsonObject>>(masaStackConfig.GetValue(MasaStackConfigConstant.MASA_SERVER)) ?? new();
    }

    public static Dictionary<string, JsonObject> GetAllUI(this IMasaStackConfig masaStackConfig)
    {
        return JsonSerializer.Deserialize<Dictionary<string, JsonObject>>(masaStackConfig.GetValue(MasaStackConfigConstant.MASA_UI)) ?? new();
    }

    public static bool HasAlert(this IMasaStackConfig masaStackConfig)
    {
        return GetAllServer(masaStackConfig).ContainsKey(MasaStackConstant.ALERT);
    }

    public static bool HasTsc(this IMasaStackConfig masaStackConfig)
    {
        return GetAllServer(masaStackConfig).ContainsKey(MasaStackConstant.TSC);
    }

    public static bool HasScheduler(this IMasaStackConfig masaStackConfig)
    {
        return GetAllServer(masaStackConfig).ContainsKey(MasaStackConstant.SCHEDULER);
    }

    public static string GetConnectionString(this IMasaStackConfig masaStackConfig, string datebaseName)
    {
        var connStr = masaStackConfig.GetValue(MasaStackConfigConstant.CONNECTIONSTRING);
        var dbModel = JsonSerializer.Deserialize<DbModel>(connStr);

        return dbModel?.ToString(datebaseName) ?? "";
    }

    public static string GetServerDomain(this IMasaStackConfig masaStackConfig, string protocol, string project, string service)
    {
        var domain = "";
        GetAllServer(masaStackConfig).TryGetValue(project, out JsonObject? jsonObject);
        if (jsonObject != null)
        {
            var secondaryDomain = jsonObject[service]?.ToString();
            if (secondaryDomain != null)
            {
                domain = $"{protocol}://{secondaryDomain}.{masaStackConfig.Namespace}";
            }
        }
        return domain;
    }

    public static string GetUIDomain(this IMasaStackConfig masaStackConfig, string protocol, string project, string service)
    {
        var domain = "";
        GetAllUI(masaStackConfig).TryGetValue(project, out JsonObject? jsonObject);
        if (jsonObject != null)
        {
            var secondaryDomain = jsonObject[service]?.ToString();
            if (secondaryDomain != null)
            {
                domain = $"{protocol}://{secondaryDomain}.{masaStackConfig.DomainName.TrimStart('.')}";
            }
        }
        return domain;
    }

    public static string GetAuthServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetServerDomain(masaStackConfig, HttpProtocol.HTTP, MasaStackConstant.AUTH, MasaStackConstant.SERVER);
    }

    public static string GetPmServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetServerDomain(masaStackConfig, HttpProtocol.HTTP, MasaStackConstant.PM, MasaStackConstant.SERVER);
    }

    public static string GetDccServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetServerDomain(masaStackConfig, HttpProtocol.HTTP, MasaStackConstant.DCC, MasaStackConstant.SERVER);
    }

    public static string GetTscServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetServerDomain(masaStackConfig, HttpProtocol.HTTP, MasaStackConstant.TSC, MasaStackConstant.SERVER);
    }

    public static string GetAlertServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetServerDomain(masaStackConfig, HttpProtocol.HTTP, MasaStackConstant.ALERT, MasaStackConstant.SERVER);
    }

    public static string GetMcServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetServerDomain(masaStackConfig, HttpProtocol.HTTP, MasaStackConstant.MC, MasaStackConstant.SERVER);
    }

    public static string GetSchedulerServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetServerDomain(masaStackConfig, HttpProtocol.HTTP, MasaStackConstant.SCHEDULER, MasaStackConstant.SERVER);
    }

    public static string GetSchedulerWorkerDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetServerDomain(masaStackConfig, HttpProtocol.HTTP, MasaStackConstant.SCHEDULER, MasaStackConstant.WORKER);
    }

    public static string GetSsoDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetUIDomain(masaStackConfig, HttpProtocol.HTTPS, MasaStackConstant.AUTH, MasaStackConstant.SSO);
    }

    public static IEnumerable<KeyValuePair<string, List<string>>> GetAllUINames(this IMasaStackConfig masaStackConfig)
    {
        foreach (var server in GetAllUI(masaStackConfig))
        {
            var uiName = server.Value[MasaStackConstant.UI]?.ToString();
            if (string.IsNullOrEmpty(uiName))
            {
                continue;
            }
            yield return new KeyValuePair<string, List<string>>(uiName, new List<string> {
                GetUIDomain(masaStackConfig,HttpProtocol.HTTP,server.Key,MasaStackConstant.UI),
                GetUIDomain(masaStackConfig,HttpProtocol.HTTPS,server.Key,MasaStackConstant.UI)
            });
        }
    }

    public static string GetServerId(this IMasaStackConfig masaStackConfig, string project, string service = MasaStackConstant.SERVER)
    {
        return masaStackConfig.GetAllServer()[project][service]?.ToString() ?? throw new KeyNotFoundException();
    }

    public static string GetWebId(this IMasaStackConfig masaStackConfig, string project, string service = MasaStackConstant.UI)
    {
        return masaStackConfig.GetAllUI()[project][service]?.ToString() ?? throw new KeyNotFoundException();
    }

    public static T GetDccMiniOptions<T>(this IMasaStackConfig masaStackConfig)
    {
        var dccServerAddress = GetDccServiceDomain(masaStackConfig);
        var redis = masaStackConfig.RedisModel ?? throw new Exception("redis options can not null");

        var stringBuilder = new System.Text.StringBuilder(@"{""ManageServiceAddress"":");
        stringBuilder.Append($"\"{dccServerAddress}\",");
        stringBuilder.Append(@"""RedisOptions"": {""Servers"": [{""Host"": ");
        stringBuilder.Append($"\"{redis.RedisHost}\",");
        stringBuilder.Append(@$"""Port"":{redis.RedisPort}");
        stringBuilder.Append("}],");
        stringBuilder.Append(@$"""DefaultDatabase"":{redis.RedisDb},");
        stringBuilder.Append(@$"""Password"": ""{redis.RedisPassword}""");
        stringBuilder.Append(@"},");
        stringBuilder.Append(@"""ConfigObjectSecret"":");
        stringBuilder.Append($"\"{masaStackConfig.DccSecret}\",");
        stringBuilder.Append(@"""PublicSecret"":");
        stringBuilder.Append($"\"{masaStackConfig.DccSecret}\"");
        stringBuilder.Append(@"}");
        return JsonSerializer.Deserialize<T>(stringBuilder.ToString()) ?? throw new JsonException();
    }

    public static Guid GetDefaultUserId(this IMasaStackConfig masaStackConfig)
    {
        return CreateGuid(masaStackConfig.Namespace);
    }

    public static Guid GetDefaultTeamId(this IMasaStackConfig masaStackConfig)
    {
        return CreateGuid(masaStackConfig.Namespace + " team");
    }

    static Guid CreateGuid(string str)
    {
#pragma warning disable S4790
        using var md5 = MD5.Create();
#pragma warning restore S4790
        byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(str));
        return new Guid(hash);
    }
}

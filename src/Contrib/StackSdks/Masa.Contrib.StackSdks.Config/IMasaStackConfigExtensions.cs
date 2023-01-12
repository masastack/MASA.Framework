// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Config;

public static class IMasaStackConfigExtensions
{
    public static Dictionary<string, List<string>> GetAllServer(this IMasaStackConfig masaStackConfig)
    {
        return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(masaStackConfig.GetValue(MasaStackConfigConst.MASA_ALL_SERVER)) ?? new();
    }

    public static bool HasAlert(this IMasaStackConfig masaStackConfig)
    {
        return GetAllServer(masaStackConfig).ContainsKey("");
    }

    public static bool HasTsc(this IMasaStackConfig masaStackConfig)
    {
        return GetAllServer(masaStackConfig).ContainsKey("");
    }

    public static bool HasScheduler(this IMasaStackConfig masaStackConfig)
    {
        return GetAllServer(masaStackConfig).ContainsKey("");
    }

    public static string GetConnectionString(this IMasaStackConfig masaStackConfig, string datebaseName)
    {
        var connStr = masaStackConfig.GetValue(MasaStackConfigConst.CONNECTIONSTRING);
        var dbModel = JsonSerializer.Deserialize<DbModel>(connStr);

        return dbModel?.ToString(datebaseName) ?? "";
    }
}

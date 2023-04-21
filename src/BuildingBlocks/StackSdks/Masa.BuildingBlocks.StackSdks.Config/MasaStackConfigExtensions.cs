// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Config;

public static class MasaStackConfigExtensions
{
    static string systemUer = "system";

    public static string GetDefaultUserName(this IMasaStackConfig masaStackConfig)
    {
        return systemUer;
    }

    public static Guid GetDefaultUserId(this IMasaStackConfig masaStackConfig)
    {
        return CreateGuid(masaStackConfig.Namespace);
    }

    public static Guid GetDefaultTeamId(this IMasaStackConfig masaStackConfig)
    {
        return CreateGuid($"{masaStackConfig.Namespace} team");
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

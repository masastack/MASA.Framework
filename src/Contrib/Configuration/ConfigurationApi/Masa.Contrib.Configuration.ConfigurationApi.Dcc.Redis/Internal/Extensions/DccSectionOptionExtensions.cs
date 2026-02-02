// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Redis.Internal.Extensions;

internal static class DccSectionOptionExtensions
{
    public static void ComplementConfigObjects(this DccSectionOptions dccSection, IDistributedCacheClient distributedCacheClient)
    {
        if (dccSection.ConfigObjects == null || dccSection.ConfigObjects.Count == 0)
        {
            dccSection.ConfigObjects = distributedCacheClient.GetAllConfigObjects(dccSection.AppId, dccSection.Environment, dccSection.Cluster);
        }
    }
}

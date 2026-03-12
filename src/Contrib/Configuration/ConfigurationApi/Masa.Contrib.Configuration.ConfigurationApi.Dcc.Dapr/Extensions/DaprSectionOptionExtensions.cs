// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Redis.Internal.Extensions;

internal static class DaprSectionOptionExtensions
{
    public static void ComplementConfigObjects(this DccDaprOptions dccSection, DaprClient client)
    {
        if (dccSection.ConfigObjects == null || dccSection.ConfigObjects.Count == 0)
        {
            dccSection.ConfigObjects = client.GetAllConfigObjects(dccSection.StoreName, dccSection.AppId, dccSection.Environment, dccSection.Cluster, dccSection.Prefix);
        }
    }
}

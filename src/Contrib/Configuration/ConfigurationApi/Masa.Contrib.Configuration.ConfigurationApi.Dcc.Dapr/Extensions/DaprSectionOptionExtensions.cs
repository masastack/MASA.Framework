// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Dapr.Internal.Extensions;

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Dapr.Extensions;

internal static class DaprSectionOptionExtensions
{
    public static void ComplementConfigObjects(this DccSectionOptions section, DaprClient client, string storeName, string prefix)
    {
        if (section.ConfigObjects.Count != 0)
            return;

        section.ConfigObjects = client.GetAllConfigObjects(storeName, section.AppId, section.Environment, section.Cluster, prefix);
    }
}

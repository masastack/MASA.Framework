// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal.Model;

internal static class StaticConfig
{
    public static string AppId { get; set; }

    public static string PublicId => "public-$Config";
}

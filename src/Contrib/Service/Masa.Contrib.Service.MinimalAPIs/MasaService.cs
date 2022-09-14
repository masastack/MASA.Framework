// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs;

public static class MasaService
{
    public static bool DisableRestful { get; set; }

    public static string Prefix { get; set; }

    public static string Version { get; set; }

    static MasaService()
    {
        DisableRestful = false;
        Prefix = string.Empty;
        Version = string.Empty;
    }
}

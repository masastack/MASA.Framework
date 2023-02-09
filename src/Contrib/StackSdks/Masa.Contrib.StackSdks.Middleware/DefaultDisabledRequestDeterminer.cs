// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Middleware;

internal class DefaultDisabledRequestDeterminer : IDisabledRequestDeterminer
{
    readonly IMasaStackConfig _masaStackConfig;

    public DefaultDisabledRequestDeterminer(IMasaStackConfig masaStackConfig)
    {
        _masaStackConfig = masaStackConfig;
    }

    public bool Determiner()
    {
        return _masaStackConfig.IsDemo;
    }
}

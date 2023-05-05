// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Middleware;

public class DefaultDisabledEventDeterminer : IDisabledEventDeterminer
{
    readonly IUserContext _userContext;
    readonly IMasaStackConfig _masaStackConfig;

    public DefaultDisabledEventDeterminer(IUserContext userContext, IMasaStackConfig masaStackConfig)
    {
        _userContext = userContext;
        _masaStackConfig = masaStackConfig;
    }

    public bool DisabledCommand => true;

    public bool Determiner()
    {
        var account = _userContext.GetUser<MasaUser>()?.Account?.ToLower();
        return _masaStackConfig.IsDemo && account != "admin" && !account.IsNullOrEmpty();
    }
}

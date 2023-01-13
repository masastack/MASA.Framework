// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient;

public class MasaDaprClient : MasaCallerClient
{
    private string _appId = default!;

    public string AppId
    {
        get => _appId;
        set
        {
            MasaArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(AppId));

            _appId = value;
        }
    }

    internal MasaDaprClient()
    {
    }

    public MasaDaprClient(string appid)
    {
        AppId = appid;
    }
}

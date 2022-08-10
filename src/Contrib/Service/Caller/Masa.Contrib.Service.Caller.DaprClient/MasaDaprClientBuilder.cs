// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient;

public class MasaDaprClientBuilder
{
    private string _appId = default!;

    public string AppId
    {
        get
        {
            return _appId;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(AppId));

            _appId = value;
        }
    }

    public Action<DaprClientBuilder>? Configure { get; set; }

    internal MasaDaprClientBuilder()
    {
    }

    public MasaDaprClientBuilder(string appid)
        : this(appid, null)
    {
    }

    public MasaDaprClientBuilder(string appid, Action<DaprClientBuilder>? configure)
    {
        AppId = appid;
        Configure = configure;
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.DaprClient;

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

    private string _name = default!;

    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(Name));

            _name = value;
        }
    }

    public bool IsDefault { get; set; } = false;

    public Action<DaprClientBuilder>? Configure { get; set; }

    internal MasaDaprClientBuilder()
    {
        this.Name = string.Empty;
    }

    public MasaDaprClientBuilder(string appid)
        : this(appid, "dapr") { }

    public MasaDaprClientBuilder(string appid, string name)
        : this(appid, name, null)
    {
    }

    public MasaDaprClientBuilder(string appid, string name, Action<DaprClientBuilder>? configure) : this(appid, name, configure, false)
    {
    }

    public MasaDaprClientBuilder(string appid, string name, Action<DaprClientBuilder>? configure, bool isDefault)
    {
        AppId = appid;
        Name = name;
        Configure = configure;
        IsDefault = isDefault;
    }
}

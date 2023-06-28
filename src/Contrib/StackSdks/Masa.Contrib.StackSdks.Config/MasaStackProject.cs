// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Config;

public class MasaStackProject : Enumeration
{
    public static readonly MasaStackProject Auth = new(1, nameof(Auth).ToLowerInvariant());
    public static readonly MasaStackProject PM = new(2, nameof(PM).ToLowerInvariant());
    public static readonly MasaStackProject DCC = new(3, nameof(DCC).ToLowerInvariant());
    public static readonly MasaStackProject MC = new(4, nameof(MC).ToLowerInvariant());
    public static readonly MasaStackProject Alert = new(5, nameof(Alert).ToLowerInvariant());
    public static readonly MasaStackProject Scheduler = new(6, nameof(Scheduler).ToLowerInvariant());
    public static readonly MasaStackProject TSC = new(7, nameof(TSC).ToLowerInvariant());

    public MasaStackProject(int id, string name)
        : base(id, name)
    {
    }
}

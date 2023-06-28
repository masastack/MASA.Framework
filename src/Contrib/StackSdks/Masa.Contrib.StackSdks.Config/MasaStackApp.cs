// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Config;

public class MasaStackApp : Enumeration
{
    static readonly MasaStackApp WEB = new(1, nameof(WEB).ToLowerInvariant());
    static readonly MasaStackApp SSO = new(2, nameof(SSO).ToLowerInvariant());
    static readonly MasaStackApp Service = new(3, nameof(Service).ToLowerInvariant());
    static readonly MasaStackApp Worker = new(4, nameof(Worker).ToLowerInvariant());

    MasaStackApp(int id, string name)
        : base(id, name)
    {
    }
}

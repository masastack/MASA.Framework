// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Config;

public class MasaStackApp : Enumeration
{
    public static MasaStackApp WEB = new(1, nameof(WEB).ToLowerInvariant());
    public static MasaStackApp SSO = new(2, nameof(SSO).ToLowerInvariant());
    public static MasaStackApp Service = new(3, nameof(Service).ToLowerInvariant());
    public static MasaStackApp Worker = new(4, nameof(Worker).ToLowerInvariant());

    MasaStackApp(int id, string name)
        : base(id, name)
    {
    }
}

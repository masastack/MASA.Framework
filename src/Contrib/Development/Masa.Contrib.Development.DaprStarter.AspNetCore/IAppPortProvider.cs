// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter.AspNetCore;

public interface IAppPortProvider
{
    bool GetEnableSsl(ushort appPort);

    (bool EnableSsl, ushort AppPort) GetAppPort(bool? enableSsl);
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Development.Dapr.AspNetCore;

public interface IAppPortProvider
{
    (bool EnableSsl, ushort AppPort) GetAppPort(bool? enableSsl);
}

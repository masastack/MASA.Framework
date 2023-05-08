// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter.AspNetCore;

public interface IAvailabilityPortProvider
{
    ushort? GetAvailablePort(ushort startingPort, IEnumerable<int>? reservedPorts = null);
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter.AspNetCore;

[ExcludeFromCodeCoverage]
public class DefaultAvailabilityPortProvider : IAvailabilityPortProvider
{
    private readonly ILogger<DefaultAppPortProvider>? _logger;

    public DefaultAvailabilityPortProvider(ILogger<DefaultAppPortProvider>? logger = null)
    {
        _logger = logger;
    }

    public ushort? GetAvailablePort(ushort startingPort, IEnumerable<int>? reservedPorts = null)
    {
        MasaArgumentException.ThrowIfGreaterThan(startingPort, ushort.MaxValue);
        try
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

            var connectionsEndpoints = ipGlobalProperties.GetActiveTcpConnections().Select(c => c.LocalEndPoint);
            var ports = connectionsEndpoints.Concat(ipGlobalProperties.GetActiveTcpListeners())
                .Concat(ipGlobalProperties.GetActiveUdpListeners())
                .Select(point => point.Port)
                .ToList();
            if (reservedPorts != null)
            {
                ports.AddRange(reservedPorts);
            }

            return (ushort)Enumerable.Range(startingPort, ushort.MaxValue - startingPort + 1).Except(ports).FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to get available ports, startingPort: {StartingPort}", startingPort);
            return null;
        }
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Development.DaprStarter.AspNetCore;

internal static class PortUtils
{
    public static void CheckCompletionPort(DaprOptions daprOptions, IServiceProvider serviceProvider)
    {
        var daprEnvironmentProvider = serviceProvider.GetRequiredService<IDaprEnvironmentProvider>();

        daprOptions.DaprHttpPort ??= daprEnvironmentProvider.GetHttpPort();
        daprOptions.DaprGrpcPort ??= daprEnvironmentProvider.GetGrpcPort();

        var reservedPorts = new List<int>();
        AddReservedPorts(daprOptions.DaprHttpPort);
        AddReservedPorts(daprOptions.DaprGrpcPort);
        AddReservedPorts(daprOptions.MetricsPort);
        AddReservedPorts(daprOptions.ProfilePort);
        AddReservedPorts(daprOptions.AppPort);

        var availabilityPortProvider = serviceProvider.GetRequiredService<IAvailabilityPortProvider>();

        daprEnvironmentProvider.TrySetHttpPort(GetPortAndAddReservedPortsByAvailability(daprOptions.DaprHttpPort, 3500));
        daprEnvironmentProvider.TrySetGrpcPort(GetPortAndAddReservedPortsByAvailability(daprOptions.DaprGrpcPort, 5001));
        daprEnvironmentProvider.TrySetMetricsPort(GetPortAndAddReservedPortsByAvailability(daprOptions.MetricsPort, 9090));

        // Environment variables need to be improved
        bool IsAvailablePort([NotNullWhen(true)] ushort? port)
        {
            return port is > 0;
        }

        void AddReservedPorts(ushort? port)
        {
            if (port is > 0) reservedPorts.Add(port.Value);
        }

        ushort? GetPortAndAddReservedPortsByAvailability(ushort? port, ushort startingPort)
        {
            ushort? portByAvailability;
            if (IsAvailablePort(port))
            {
                portByAvailability = port.Value;
            }
            else
            {
                portByAvailability = availabilityPortProvider.GetAvailablePort(startingPort, reservedPorts);
                if (portByAvailability != null) reservedPorts.Add(portByAvailability.Value);
            }

            return portByAvailability;
        }
    }
}

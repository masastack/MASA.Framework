// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter.AspNetCore;

public class DefaultAppPortProvider : IAppPortProvider
{
    private readonly IServer _server;

    public DefaultAppPortProvider(IServer server) => _server = server;

    public bool GetEnableSsl(ushort appPort)
    {
        var ports = GetPorts();
        if (ports.Any(p => p.Port == appPort))
        {
            var port = ports.First(p => p.Port == appPort);
            return port.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
        }

        throw new UserFriendlyException($"The current port {appPort} is unavailable, Dapr failed to start");
    }

    public (bool EnableSsl, ushort AppPort) GetAppPort(bool? enableSsl)
    {
        var ports = GetPorts();

        if (ports.Count == 1)
        {
            return new(ports[0].Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase), (ushort)ports[0].Item2);
        }

        if (enableSsl is false && ports.Any(p => p.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)))
        {
            return new(false, GetAppPort(ports, false));
        }

        return new(true, GetAppPort(ports, true));
    }

    private List<(string Scheme, int Port)> GetPorts()
    {
        var addresses = _server.Features.Get<IServerAddressesFeature>()?.Addresses;
        if (addresses is { IsReadOnly: false, Count: 0 })
            throw new UserFriendlyException("Failed to get the startup port, please specify the port manually");

        var ports = addresses!
            .Select(address => new Uri(address))
            .Where(address
                => address.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
                   || address.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
            .Select(address => new ValueTuple<string, int>(address.Scheme, address.Port)).ToList();
        return ports;
    }

    public static ushort GetAppPort(List<ValueTuple<string, int>> ports, bool enableSsl)
    {
        if (enableSsl)
        {
            return ports
                .Where(p => p.Item1.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                .Select(p => (ushort)p.Item2)
                .FirstOrDefault();
        }

        return ports
            .Where(p => p.Item1.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
            .Select(p => (ushort)p.Item2)
            .FirstOrDefault();
    }
}

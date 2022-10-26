// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarters.AspNetCore;

public class DefaultAppPortProvider : IAppPortProvider
{
    private readonly IServer _server;

    public DefaultAppPortProvider(IServer server) => _server = server;

    public (bool EnableSsl, ushort AppPort) GetAppPort(bool? enableSsl)
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

        if (ports.Count == 1)
        {
            return new(ports[0].Item1.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase), (ushort)ports[0].Item2);
        }

        if (enableSsl is true && ports.Any(p => p.Item1.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)))
        {
            return new(true, GetAppPort(ports, true));
        }
        return new(false, GetAppPort(ports, false));
    }

    private static ushort GetAppPort(List<ValueTuple<string, int>> ports, bool enableSsl)
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

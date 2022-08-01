// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Development.Dapr.AspNetCore;

public class DefaultAppPortProvider : IAppPortProvider
{
    private readonly IServer _server;

    public DefaultAppPortProvider(IServer server) => _server = server;

    public ushort GetAppPort(bool? enableSsl)
    {
        var addresses = _server.Features.Get<IServerAddressesFeature>()?.Addresses;
        if (addresses is { IsReadOnly: false, Count: 0 })
            throw new Exception("Failed to get the startup port, please specify the port manually");

        var ports = addresses!
            .Select(address => new Uri(address))
            .Where(address
                => (enableSsl is true && address.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                || address.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
            .Select(address => new
            {
                address.Scheme,
                address.Port
            }).ToList();

        if (enableSsl is true)
        {
            return ports
                .Where(p => p.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                .Select(p => (ushort)p.Port)
                .FirstOrDefault();
        }

        return ports
            .Where(p => p.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
            .Select(p => (ushort)p.Port)
            .FirstOrDefault();
    }
}

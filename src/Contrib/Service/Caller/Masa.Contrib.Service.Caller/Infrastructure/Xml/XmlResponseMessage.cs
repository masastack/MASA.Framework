// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Service.Caller;

public class XmlResponseMessage : DefaultResponseMessage
{
    public XmlResponseMessage(ILoggerFactory? loggerFactory = null)
        : base(loggerFactory?.CreateLogger<DefaultResponseMessage>())
    {
    }

    protected override async Task<TResponse?> FormatResponseAsync<TResponse>(HttpContent httpContent,
        CancellationToken cancellationToken = default) where TResponse : default
    {
        try
        {
            var content = await httpContent.ReadAsStringAsync(cancellationToken);
            return XmlUtils.Deserialize<TResponse>(content);
        }
        catch (Exception exception)
        {
            Logger?.LogWarning(exception, "{Message}", exception.Message);
            ExceptionDispatchInfo.Capture(exception).Throw();
            return default; //This will never be executed, the previous line has already thrown an exception
        }
    }
}

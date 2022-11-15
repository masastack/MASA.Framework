// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller;

public class JsonResponseMessage : DefaultResponseMessage
{
    public JsonResponseMessage(
        IOptions<CallerFactoryOptions> options,
        ILoggerFactory? loggerFactory = null) : base(options, loggerFactory?.CreateLogger<DefaultResponseMessage>())
    {
    }

    protected override async Task<TResponse?> FormatResponseAsync<TResponse>(HttpContent httpContent,
        CancellationToken cancellationToken = default) where TResponse : default
    {
        try
        {
            return await httpContent.ReadFromJsonAsync<TResponse>(
                Options.JsonSerializerOptions ?? MasaApp.GetJsonSerializerOptions()
                , cancellationToken);
        }
        catch (Exception exception)
        {
            Logger?.LogWarning(exception, "{Message}", exception.Message);
            ExceptionDispatchInfo.Capture(exception).Throw();
            return default; //This will never be executed, the previous line has already thrown an exception
        }
    }
}

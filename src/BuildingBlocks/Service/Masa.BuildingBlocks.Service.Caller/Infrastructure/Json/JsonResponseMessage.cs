// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class JsonResponseMessage : DefaultResponseMessage
{
    private readonly JsonSerializerOptions? _jsonSerializerOptions;

    public JsonResponseMessage(
        JsonSerializerOptions? jsonSerializerOptions,
        ILoggerFactory? loggerFactory = null) : base(loggerFactory?.CreateLogger<DefaultResponseMessage>())
    {
        _jsonSerializerOptions = jsonSerializerOptions ?? MasaApp.GetJsonSerializerOptions();
    }

    protected override Task<TResponse?> FormatResponseAsync<TResponse>(HttpContent httpContent,
        CancellationToken cancellationToken = default) where TResponse : default
    {
        try
        {
            return httpContent.ReadFromJsonAsync<TResponse>(_jsonSerializerOptions, cancellationToken);
        }
        catch (Exception exception)
        {
            Logger?.LogWarning(exception, "{Message}", exception.Message);
            ExceptionDispatchInfo.Capture(exception).Throw();
            return default; //This will never be executed, the previous line has already thrown an exception
        }
    }
}

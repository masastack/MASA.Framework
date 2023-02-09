// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public static class MasaCallerOptionsExtensions
{
    /// <summary>
    /// Set the request handler and response handler for the specified Caller
    /// </summary>
    /// <param name="masaCallerOptions"></param>
    /// <param name="jsonSerializerOptions"></param>
    /// <returns></returns>
    public static MasaCallerClient UseJson(this MasaCallerClient masaCallerOptions, JsonSerializerOptions? jsonSerializerOptions)
    {
        masaCallerOptions.RequestMessageFactory = _ => new JsonRequestMessage(jsonSerializerOptions);
        masaCallerOptions.ResponseMessageFactory = serviceProvider =>
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            return new JsonResponseMessage(jsonSerializerOptions, loggerFactory);
        };
        return masaCallerOptions;
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Authentication.Identity.Core")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.Serialization.Json;

internal static class JsonSerializerOptionsHelper
{
    public static JsonSerializerOptions? GetJsonSerializerOptions(
        IServiceProvider serviceProvider,
        JsonSerializerOptions? jsonSerializerOptions)
    {
        return jsonSerializerOptions ??
            serviceProvider.GetService<IOptionsFactory<JsonSerializerOptions>>()?.Create(Options.DefaultName) ??
            MasaApp.GetJsonSerializerOptions();
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class SerializationBuilderExtensions
{
    public static void UseJson(this SerializationBuilder serializationBuilder, Action<JsonSerializerOptions>? configure = null)
    {
        JsonSerializerOptions? jsonSerializerOptions = null;
        if (configure != null)
        {
            jsonSerializerOptions = new JsonSerializerOptions();
            configure.Invoke(jsonSerializerOptions);
        }

        serializationBuilder.Services.TryAddJsonCore(jsonSerializerOptions);
        serializationBuilder.UseSerialization(
            serviceProvider => new DefaultJsonSerializer(JsonSerializerOptionsHelper.GetJsonSerializerOptions(serviceProvider, jsonSerializerOptions)),
            serviceProvider => new DefaultJsonDeserializer(JsonSerializerOptionsHelper.GetJsonSerializerOptions(serviceProvider, jsonSerializerOptions)));
    }

    private static void TryAddJsonCore(this IServiceCollection services, JsonSerializerOptions? jsonSerializerOptions)
    {
        services.TryAddSingleton<IJsonSerializer>(serviceProvider
            => new DefaultJsonSerializer(JsonSerializerOptionsHelper.GetJsonSerializerOptions(serviceProvider, jsonSerializerOptions)));

        services.TryAddSingleton<IJsonDeserializer>(serviceProvider
            => new DefaultJsonDeserializer(JsonSerializerOptionsHelper.GetJsonSerializerOptions(serviceProvider, jsonSerializerOptions)));
    }
}

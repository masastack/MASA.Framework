// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs.Memory.Extensions;

public static class BackgroundJobOptionsBuilderExtensions
{
    public static void UseInMemoryDatabase(
        this BackgroundJobOptionsBuilder backgroundJobOptionsBuilder)
    {
        backgroundJobOptionsBuilder.UseInMemoryDatabase(_ =>
        {
        }, serviceProvider =>
        {
            var idGenerator = serviceProvider.GetService<IIdGenerator<Guid>>();
            return idGenerator ?? new NormalGuidGenerator();
        });
    }

    public static void UseInMemoryDatabase(
        this BackgroundJobOptionsBuilder backgroundJobOptionsBuilder,
        Action<BackgroundJobOptions> configure,
        Func<IServiceProvider, IIdGenerator<Guid>> idGeneratorFunc)
    {
        backgroundJobOptionsBuilder.UseInMemoryDatabase(
            configure,
            idGeneratorFunc,
            serviceProvider => serviceProvider.GetService<IJsonSerializer>() ?? new DefaultJsonSerializer(MasaApp.GetJsonSerializerOptions()),
            serviceProvider => serviceProvider.GetService<IJsonDeserializer>() ?? new DefaultJsonDeserializer(MasaApp.GetJsonSerializerOptions()));
    }

    public static void UseInMemoryDatabase(
        this BackgroundJobOptionsBuilder backgroundJobOptionsBuilder,
        Action<BackgroundJobOptions> configure,
        Func<IServiceProvider, IIdGenerator<Guid>> idGeneratorFunc,
        Func<IServiceProvider, ISerializer> serializerFunc,
        Func<IServiceProvider, IDeserializer> deserializerFunc)
    {
        backgroundJobOptionsBuilder.UseBackgroundJobCore(configure, idGeneratorFunc, serializerFunc, deserializerFunc);
        backgroundJobOptionsBuilder.Services.TryAddSingleton<IBackgroundJobStorage, BackgroundJobStorage>();
    }
}

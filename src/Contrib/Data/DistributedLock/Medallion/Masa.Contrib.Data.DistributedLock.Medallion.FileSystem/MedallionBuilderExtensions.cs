// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class MedallionBuilderExtensions
{
    public static void UseFileSystem(this MedallionBuilder medallionBuilder, string path)
        => medallionBuilder.UseFileSystem(new DirectoryInfo(path));

    public static void UseFileSystem(this MedallionBuilder medallionBuilder, DirectoryInfo lockFileDirectory)
    {
        medallionBuilder.Services.AddSingleton<IDistributedLockProvider>(_
            => new FileDistributedSynchronizationProvider(lockFileDirectory));
    }
}

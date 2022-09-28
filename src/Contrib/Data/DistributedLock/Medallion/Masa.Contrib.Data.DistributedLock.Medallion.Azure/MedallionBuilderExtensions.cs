// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class MedallionBuilderExtensions
{
    public static void UseAzure(this MedallionBuilder medallionBuilder,
        string connectionString,
        string blobContainerName,
        Action<AzureBlobLeaseOptionsBuilder>? options = null)
        => medallionBuilder.UseAzure(new BlobContainerClient(connectionString, blobContainerName), options);

    public static void UseAzure(this MedallionBuilder medallionBuilder,
        string connectionString,
        string blobContainerName,
        BlobClientOptions blobClientOptions,
        Action<AzureBlobLeaseOptionsBuilder>? options = null)
        => medallionBuilder.UseAzure(new BlobContainerClient(connectionString, blobContainerName, blobClientOptions), options);

    public static void UseAzure(this MedallionBuilder medallionBuilder,
        BlobContainerClient blobContainerClient,
        Action<AzureBlobLeaseOptionsBuilder>? options = null)
    {
        medallionBuilder.Services.AddSingleton<IDistributedLockProvider>(_
            => new AzureBlobLeaseDistributedSynchronizationProvider(blobContainerClient, options));
    }
}

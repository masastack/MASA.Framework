// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public class DefaultClientFactory : IClientFactory
{
    private readonly IClient _client;

    public DefaultClientFactory(IClient client) => _client = client;

    public IClientContainer Create(string bucketName)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
            throw new AggregateException($"{nameof(bucketName)} cannot be empty");

        return new DefaultClientContainer(_client, bucketName);
    }
}

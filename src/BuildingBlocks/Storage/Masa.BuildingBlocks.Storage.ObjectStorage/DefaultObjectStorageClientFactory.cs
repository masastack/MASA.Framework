// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public class DefaultObjectStorageClientFactory : MasaFactoryBase<IManualObjectStorageClient, MasaRelationOptions<IManualObjectStorageClient>>, IObjectStorageClientFactory
{
    protected override string DefaultServiceNotFoundMessage => "No default ObjectStorage found";
    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{0}] ObjectStorage, it was not found";

    protected override MasaFactoryOptions<MasaRelationOptions<IManualObjectStorageClient>> FactoryOptions => _options.CurrentValue;

    private readonly IOptionsMonitor<ObjectStorageFactoryOptions> _options;

    public DefaultObjectStorageClientFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _options = serviceProvider.GetRequiredService<IOptionsMonitor<ObjectStorageFactoryOptions>>();
    }
}

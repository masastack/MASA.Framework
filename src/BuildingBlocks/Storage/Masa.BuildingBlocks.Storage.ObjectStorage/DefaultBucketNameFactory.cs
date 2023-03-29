// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public class DefaultBucketNameFactory : MasaFactoryBase<IBucketNameProvider, MasaRelationOptions<IBucketNameProvider>>, IBucketNameFactory
{
    protected override string DefaultServiceNotFoundMessage => "No default ObjectStorageBucketName found";
    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{0}] ObjectStorageBucketName, it was not found";
    protected override MasaFactoryOptions<MasaRelationOptions<IBucketNameProvider>> FactoryOptions => _options.CurrentValue;

    private readonly IOptionsMonitor<BucketNameFactoryOptions> _options;

    public DefaultBucketNameFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _options = serviceProvider.GetRequiredService<IOptionsMonitor<BucketNameFactoryOptions>>();
    }
}

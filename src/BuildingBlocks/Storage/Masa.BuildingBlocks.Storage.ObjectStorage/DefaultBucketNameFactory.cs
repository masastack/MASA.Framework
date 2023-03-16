// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public class DefaultBucketNameFactory : MasaFactoryBase<IBucketNameProvider, BucketNameRelationOptions>, IBucketNameFactory
{
    protected override string DefaultServiceNotFoundMessage => "No default ObjectStorageBucketName found";
    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{0}] ObjectStorageBucketName, it was not found";
    protected override MasaFactoryOptions<BucketNameRelationOptions> FactoryOptions => _options.CurrentValue;

    private readonly IOptionsMonitor<BucketNameFactoryOptions> _options;

    public DefaultBucketNameFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _options = serviceProvider.GetRequiredService<IOptionsMonitor<BucketNameFactoryOptions>>();
    }

    protected override IServiceProvider GetServiceProvider(string name)
    {
        var options = TransientServiceProvider.GetRequiredService<IOptions<IsolationOptions>>();
        if (options.Value is { Enable: true })
            return ScopedServiceProvider;

        return SingletonServiceProvider;
    }
}

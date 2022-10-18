// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Localization;

public class MasaStringLocalizerFactory : IMasaStringLocalizerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public MasaStringLocalizerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IMasaStringLocalizer Create<TResourceSource>()
        => _serviceProvider.GetRequiredService<IMasaStringLocalizer<TResourceSource>>();

    public IMasaStringLocalizer Create(Type resourceSource)
    {
        var serviceType = typeof(IMasaStringLocalizer<>).MakeGenericType(resourceSource);
        return (IMasaStringLocalizer)_serviceProvider.GetRequiredService(serviceType);
    }
}

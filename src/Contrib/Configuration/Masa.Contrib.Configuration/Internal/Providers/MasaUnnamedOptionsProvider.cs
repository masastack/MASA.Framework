// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal class MasaUnnamedOptionsProvider
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Lazy<HashSet<Type>> _autoMapOptionsTypesLazy;
    private HashSet<Type> AutoMapOptionsTypes => _autoMapOptionsTypesLazy.Value;

    public MasaUnnamedOptionsProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _autoMapOptionsTypesLazy = new(() => serviceProvider.GetRequiredService<AutoMapOptionsProvider>().AutoMapTypes);
    }

    public TOptions GetOptions<[DynamicallyAccessedMembers(ConfigurationConstant.DYNAMICALLY_ACCESSED_MEMBERS)] TOptions>(
        IServiceProvider serviceProvider) where TOptions : class
    {
        if (!ConfigurationUtils.IsSkipAutoOptions(typeof(TOptions)) &&
            AutoMapOptionsTypes.Contains(typeof(TOptions)) &&
            _serviceProvider.EnableMultiEnvironment())
        {
            return serviceProvider.GetRequiredService<MasaUnnamedOptionsProvider<TOptions>>().GetOptions();
        }

        // Use the default option mode, which does not support isolation
        return _serviceProvider.GetRequiredService<UnnamedOptionsManager<TOptions>>().Value;
    }
}

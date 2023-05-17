// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal class MasaOptionsMonitorProvider
{
    private readonly Lazy<HashSet<Type>> _autoMapTypesLazy;
    private  HashSet<Type> AutoMapTypes => _autoMapTypesLazy.Value;

    public MasaOptionsMonitorProvider(IServiceProvider serviceProvider)
        => _autoMapTypesLazy = new Lazy<HashSet<Type>>(() => serviceProvider.GetRequiredService<AutoMapOptionsProvider>().AutoMapTypes);

    public IOptionsMonitor<TOptions> GetOptionsMonitor<
        [DynamicallyAccessedMembers(ConfigurationConstant.DYNAMICALLY_ACCESSED_MEMBERS)]
        TOptions>(
        IServiceProvider serviceProvider)
        where TOptions : class
    {
        var enableMultiEnvironment = serviceProvider.EnableMultiEnvironment();
        if (enableMultiEnvironment && AutoMapTypes.Contains(typeof(TOptions)))
            throw new NotSupportedException($"Multi-environment mode does not support IOptionsMonitor<{typeof(TOptions).FullName}>");

        return serviceProvider.GetRequiredService<OptionsMonitor<TOptions>>();
    }
}

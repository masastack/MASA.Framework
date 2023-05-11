// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal class MasaOptionsMonitorProvider
{
    private readonly IReadOnlyList<ConfigurationRelationOptions> _relationOptions;

    public MasaOptionsMonitorProvider(IReadOnlyList<ConfigurationRelationOptions> relationOptions)
        => _relationOptions = relationOptions;

    public IOptionsMonitor<TOptions> GetOptionsMonitor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TOptions>(
        IServiceProvider serviceProvider)
        where TOptions : class
    {
        var enableMultiEnvironment = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>().Value.EnableMultiEnvironment;
        if (enableMultiEnvironment && _relationOptions.Any(options => options.ObjectType == typeof(TOptions)))
            throw new NotSupportedException($"Multi-environment mode does not support IOptionsMonitor<{typeof(TOptions).FullName}>");

        return serviceProvider.GetRequiredService<OptionsMonitor<TOptions>>();
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal class AutoMapOptionsProvider
{
    private readonly Lazy<ConfigurationAutoMapOptions> _autoOptionsLazy;

    private ConfigurationAutoMapOptions AutoOptions => _autoOptionsLazy.Value;

    private Dictionary<(Type ObjectType, string OptionsName), ConfigurationRelationOptions>? _completeAutoMapOptions;
    public Dictionary<(Type ObjectType, string OptionsName), ConfigurationRelationOptions> CompleteAutoMapOptions
        => _completeAutoMapOptions ??= AutoOptions.Data
            .Where(options => !(options.SectionType == SectionTypes.ConfigurationApi && options.ParentSection.IsNullOrWhiteSpace()))
            .ToDictionary(item => (item.ObjectType, item.OptionsName), item => item);

    private HashSet<Type>? _autoMapTypes;
    public HashSet<Type> AutoMapTypes
        => _autoMapTypes ??= new HashSet<Type>(AutoOptions.Data.Select(options => options.ObjectType));

    private Dictionary<(Type ObjectType, string OptionsName), ConfigurationRelationOptions>? _incompleteAutoMapOptions;
    public Dictionary<(Type ObjectType, string OptionsName), ConfigurationRelationOptions> IncompleteAutoMapOptions
        => _incompleteAutoMapOptions ??= AutoOptions.Data
            .Where(options => (options.SectionType == SectionTypes.ConfigurationApi && options.ParentSection.IsNullOrWhiteSpace()))
            .ToDictionary(item => (item.ObjectType, item.OptionsName), item => item);

    public AutoMapOptionsProvider(IServiceProvider serviceProvider)
    {
        _autoOptionsLazy = new(() => serviceProvider.GetRequiredService<IOptions<ConfigurationAutoMapOptions>>().Value);
    }
}

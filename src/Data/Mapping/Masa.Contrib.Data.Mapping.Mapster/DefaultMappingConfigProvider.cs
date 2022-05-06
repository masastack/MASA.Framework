// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster;

public class DefaultMappingConfigProvider : IMappingConfigProvider
{
    private readonly ConcurrentDictionary<(Type SourceType, Type DestinationType, MapOptions? MapOptions), Lazy<TypeAdapterConfig?>>
        _store = new();

    private readonly MapOptions _options;

    public DefaultMappingConfigProvider(MapOptions options) => _options = options;

    public TypeAdapterConfig GetConfig(Type sourceType, Type destinationType, MapOptions? options = null)
        => GetConfigByCache(sourceType, destinationType, options);

    protected virtual TypeAdapterConfig GetConfigByCache(Type sourceType, Type destinationType, MapOptions? options)
    {
        TypeAdapterConfig? config = _store.GetOrAdd<(Type SourceType, Type DestinationType, MapOptions? MapOptions), TypeAdapterConfig?>(
            (sourceType, destinationType, options),
            type => GetAdapterConfig(type.SourceType, type.DestinationType, options));

        return config ?? GetDefaultConfig(options);
    }

    protected virtual TypeAdapterConfig? GetAdapterConfig(Type sourceType, Type destinationType, MapOptions? options)
    {
        TypeAdapterConfig adapterConfig = GetDefaultConfig(options);

        var mapTypes = GetMapAndSelectorTypes(adapterConfig, sourceType, destinationType, options, true);

        foreach (var item in mapTypes)
        {
            var methodExecutor = InvokeBuilder.Build(item.SourceType, item.DestinationType);
            methodExecutor.Invoke(adapterConfig, item.Constructor);
        }

        return IsShare(options) ? null : adapterConfig; //When in shared mode, Config returns empty to save memory space
    }

    //todo: In the follow-up, according to the situation, consider whether the configuration requires Fork, which is not processed for the time being
    private List<MapTypeOptions> GetMapTypes(
        TypeAdapterConfig adapterConfig,
        Type sourceType,
        Type destinationType,
        MapOptions? options)
    {
        if (!NeedAutomaticMap(sourceType, destinationType))
            return new List<MapTypeOptions>();

        List<MapTypeOptions> mapTypes = new();
        var sourceProperties = sourceType.GetProperties().ToList();
        var destinationProperties = destinationType.GetProperties().ToList();

        List<ConstructorInfo> destinationConstructors = destinationType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(c => c.GetParameters().Length <= sourceProperties.Count)
            .OrderByDescending(c => c.GetParameters().Length)
            .ToList();

        MapTypeOptions mapTypeOption = new(sourceType, destinationType)
        {
            Constructor = GetBestConstructor(destinationConstructors, sourceProperties)
        };
        if (!RuleMapIsExist(adapterConfig, sourceType, destinationType))
        {
            mapTypes.Add(mapTypeOption);
        }

        List<(string Name, Type DdestinationPropertyType)> destinationPropertyList = destinationProperties
            .Select(p => (p.Name.ToLower(), p.PropertyType))
            .Concat(mapTypeOption.Constructor.GetParameters().Select(p => (p.Name!.ToLower(), p.ParameterType))!)
            .Distinct()
            .ToList();

        foreach (var sourceProperty in sourceProperties)
        {
            if (!sourceProperty.CanRead)
                continue;

            var destinationProperty = destinationPropertyList.FirstOrDefault(p
                => p.Name.Equals(sourceProperty.Name, StringComparison.OrdinalIgnoreCase));
            if (destinationProperty != default)
            {
                var subMapTypes = GetMapAndSelectorTypes(adapterConfig, sourceProperty.PropertyType,
                    destinationProperty.DdestinationPropertyType, options, false);

                if (!subMapTypes.Any() || mapTypes.Any(option => subMapTypes.Any(subOption
                        => subOption.SourceType == option.SourceType && subOption.DestinationType == option.DestinationType)))
                    continue;

                mapTypes.AddRange(subMapTypes);
            }
        }

        return mapTypes;
    }

    private List<MapTypeOptions> GetMapAndSelectorTypes(TypeAdapterConfig adapterConfig, Type sourceType, Type destinationType,
        MapOptions? options, bool isFirst)
    {
        bool sourcePropertyIsEnumerable = IsCollection(sourceType);
        bool destinationPropertyIsEnumerable = IsCollection(destinationType);
        if (!sourcePropertyIsEnumerable && !destinationPropertyIsEnumerable)
        {
            var subMapTypes = GetMapTypes(
                adapterConfig,
                sourceType,
                destinationType,
                options);
            if (subMapTypes.Any()) return subMapTypes;
        }
        else if (sourcePropertyIsEnumerable && destinationPropertyIsEnumerable)
        {
            var subMapTypes = GetMapTypes(adapterConfig,
                sourceType.GetGenericArguments()[0],
                destinationType.GetGenericArguments()[0],
                options);

            if (subMapTypes.Any()) return subMapTypes;
        }
        return new();
    }

    protected virtual bool IsCollection(Type type)
        => type.IsGenericType && type.GetInterfaces().Any(x => x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

    protected virtual ConstructorInfo GetBestConstructor(List<ConstructorInfo> destinationConstructors, List<PropertyInfo> sourceProperties)
    {
        if (destinationConstructors.Count <= 1)
            return destinationConstructors.First();

        foreach (var constructor in destinationConstructors)
        {
            if (IsPreciseMatch(constructor, sourceProperties))
                return constructor;
        }

        throw new Exception("Failed to get the best constructor");
    }

    protected virtual bool IsPreciseMatch(ConstructorInfo destinationConstructor, List<PropertyInfo> sourceProperties)
    {
        foreach (var parameter in destinationConstructor.GetParameters())
        {
            if (!sourceProperties.Any(p
                    => p.Name.Equals(parameter.Name, StringComparison.OrdinalIgnoreCase) && p.PropertyType == parameter.ParameterType))
            {
                return false;
            }
        }
        return true;
    }

    protected virtual List<Type> NotNeedAutomaticMapTypes => new()
    {
        typeof(string)
    };

    protected virtual bool NeedAutomaticMap(Type sourceType, Type destinationType)
        => sourceType.IsClass &&
            !IsCollection(sourceType) &&
            (sourceType != destinationType || (sourceType != typeof(object) || destinationType != typeof(object))) &&
            !NotNeedAutomaticMapTypes.Contains(sourceType);

    protected virtual bool RuleMapIsExist(TypeAdapterConfig adapterConfig, Type sourceType, Type destinationType)
        => adapterConfig.RuleMap.Any(r => r.Key == new TypeTuple(sourceType, destinationType));

    protected virtual bool IsShare(MapOptions? options) => (options?.Mode ?? _options.Mode) == MapMode.Shared;

    /// <summary>
    /// Get initial configuration
    /// When currently in shared mode, return the default global settings
    /// </summary>
    /// <returns></returns>
    protected virtual TypeAdapterConfig GetDefaultConfig(MapOptions? options)
    {
        //todo: Other modes are currently not supported, and will be added in the future according to the situation
        switch (options?.Mode ?? _options.Mode)
        {
            case MapMode.Shared:
                return TypeAdapterConfig.GlobalSettings;
            default:
                throw new ArgumentException("Only shared configuration is supported", nameof(MapOptions.Mode));
        }
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Isolation;

public class EsIsolationConfigProvider
{
    readonly IMultiEnvironmentContext _multiEnvironmentContext;
    readonly EnvironmentProvider _environmentProvider;
    readonly IMultiEnvironmentMasaStackConfig _multiEnvironmentMasaStackConfig;
    readonly IMasaStackConfig _masaStackConfig;
    readonly ILogger<EsIsolationConfigProvider>? _logger;

    readonly Dictionary<string, ElasticModel> _esOptions = new();

    public EsIsolationConfigProvider(
        EnvironmentProvider environmentProvider,
        ILogger<EsIsolationConfigProvider>? logger,
        IMultiEnvironmentContext multiEnvironmentContext,
        IMultiEnvironmentMasaStackConfig multiEnvironmentMasaStackConfig,
        IMasaStackConfig masaStackConfig)
    {
        _multiEnvironmentContext = multiEnvironmentContext;
        _environmentProvider = environmentProvider;
        _multiEnvironmentMasaStackConfig = multiEnvironmentMasaStackConfig;
        _logger = logger;
        _masaStackConfig = masaStackConfig;

        InitData();
    }

    void InitData()
    {
        foreach (var envionment in _environmentProvider.GetEnvionments())
        {
            var result = _esOptions.TryAdd(envionment, _multiEnvironmentMasaStackConfig.SetEnvironment(envionment).ElasticModel);
            if (!result)
            {
                _logger?.LogWarning("Duplicate key {Envionment}", envionment);
            }
        }
    }

    public ElasticModel GetEsOptions()
    {
        if (!_esOptions.Any())
        {
            throw new UserFriendlyException("Missing es configuration, please check the configuration of MasaStackConfig");
        }
        if (_esOptions.ContainsKey(_multiEnvironmentContext.CurrentEnvironment))
        {
            return _esOptions[_multiEnvironmentContext.CurrentEnvironment];
        }
        if (_esOptions.ContainsKey(_masaStackConfig.Environment))
        {
            _logger?.LogError("The current environment does not match the es configuration, use the configuration of the _masaStackConfig.Environment {Envionment}", _masaStackConfig.Environment);
            return _esOptions[_masaStackConfig.Environment];
        }

        _logger?.LogError("No es configuration is matched, the first item is used by default");
        return _esOptions.First().Value;
    }
}

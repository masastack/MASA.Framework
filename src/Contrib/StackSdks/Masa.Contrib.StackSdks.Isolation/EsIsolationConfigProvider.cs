// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Isolation;

public class EsIsolationConfigProvider
{
    readonly IMultiEnvironmentContext _multiEnvironmentContext;
    readonly EnvironmentProvider _environmentProvider;
    readonly IMultiEnvironmentMasaStackConfig _multiEnvironmentMasaStackConfig;
    readonly ILogger<EsIsolationConfigProvider>? _logger;

    Dictionary<string, ElasticModel> _esOptions = new();

    public EsIsolationConfigProvider(
        EnvironmentProvider environmentProvider,
        ILogger<EsIsolationConfigProvider>? logger,
        IMultiEnvironmentContext multiEnvironmentContext,
        IMultiEnvironmentMasaStackConfig multiEnvironmentMasaStackConfig)
    {
        _multiEnvironmentContext = multiEnvironmentContext;
        _environmentProvider = environmentProvider;
        _multiEnvironmentMasaStackConfig = multiEnvironmentMasaStackConfig;
        _logger = logger;

        InitData();
    }

    void InitData()
    {
        foreach (var envionment in _environmentProvider.GetEnvionments())
        {
            var result = _esOptions.TryAdd(envionment, _multiEnvironmentMasaStackConfig.SetEnvironment(envionment).ElasticModel);
            if (!result)
            {
                _logger?.LogWarning($"Duplicate key {envionment}");
            }
        }
    }

    public ElasticModel GetEsOptions()
    {
        return _esOptions[_multiEnvironmentContext.CurrentEnvironment];
    }
}

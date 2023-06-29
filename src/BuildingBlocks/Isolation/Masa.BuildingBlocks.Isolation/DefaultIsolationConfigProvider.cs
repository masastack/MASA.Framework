// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Isolation;

public class DefaultIsolationConfigProvider : IIsolationConfigProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMultiEnvironmentContext? _environmentContext;
    private readonly IMultiTenantContext? _tenantContext;
    private readonly ILogger<DefaultIsolationConfigProvider>? _logger;
    private readonly List<ComponentConfigRelationInfo> _data;
    private readonly List<ComponentConfigRelationInfo> _componentConfigs;

    public DefaultIsolationConfigProvider(
        IServiceProvider serviceProvider,
        IMultiEnvironmentContext? environmentContext = null,
        IMultiTenantContext? tenantContext = null,
        ILogger<DefaultIsolationConfigProvider>? logger = null)
    {
        _serviceProvider = serviceProvider;
        _environmentContext = environmentContext;
        _tenantContext = tenantContext;
        _logger = logger;
        _data = new();
        _componentConfigs = new();
    }

    public TComponentConfig? GetComponentConfig<TComponentConfig>(string sectionName, string name = "") where TComponentConfig : class
    {
        var item = _data.FirstOrDefault(config => config.ComponentConfigType == typeof(TComponentConfig) && config.SectionName == sectionName);
        if (item != null)
            return item.Data as TComponentConfig;

        Expression<Func<IsolationConfigurationOptions<TComponentConfig>, bool>> condition = option => true;

        if (_tenantContext != null)
        {
            if (_tenantContext.CurrentTenant == null)
                _logger?.LogDebug($"Tenant resolution failed");

            condition = condition.And(option => option.TenantId == "*" || (_tenantContext.CurrentTenant != null &&
                _tenantContext.CurrentTenant.Id.Equals(option.TenantId, StringComparison.CurrentCultureIgnoreCase)));
        }

        if (_environmentContext != null)
        {
            if (string.IsNullOrEmpty(_environmentContext.CurrentEnvironment))
            {
                _logger?.LogDebug($"Environment resolution failed");
            }

            condition = condition.And(option
                => option.Environment == "*" ||
                option.Environment.Equals(_environmentContext.CurrentEnvironment, StringComparison.CurrentCultureIgnoreCase));
        }

        var data = GetIsolationConfigurationOptions<TComponentConfig>(name, sectionName);
        TComponentConfig? componentConfigInfo;
        var componentConfigs = data
            .Where(condition.Compile())
            .OrderByDescending(option => option.Score)
            .Select(option => option.Data)
            .ToList();
        if (componentConfigs.Count >= 1)
        {
            _logger?.LogDebug("{Message}, The number of matching available configurations: {Num}",
                GetMessage(),
                componentConfigs.Count);
            componentConfigInfo = componentConfigs.First();
            _logger?.LogDebug("options is {Options}", componentConfigInfo);
        }
        else
        {
            componentConfigInfo = null;
        }
        _data.Add(new ComponentConfigRelationInfo()
        {
            Data = componentConfigInfo,
            ComponentConfigType = typeof(TComponentConfig),
            SectionName = sectionName
        });
        return componentConfigInfo;
    }

    public List<TComponentConfig> GetComponentConfigs<TComponentConfig>(string sectionName, string name = "") where TComponentConfig : class
    {
        var data = GetIsolationConfigurationOptions<TComponentConfig>(name, sectionName);
        var componentConfigs = data
            .OrderByDescending(option => option.Score)
            .Select(option => option.Data)
            .ToList();
        return componentConfigs;
    }

    private List<IsolationConfigurationOptions<TComponentConfig>> GetIsolationConfigurationOptions<TComponentConfig>(
        string name,
        string sectionName)
        where TComponentConfig : class
    {
        var item = _componentConfigs.FirstOrDefault(config => config.ComponentConfigType == typeof(TComponentConfig) && config.SectionName == sectionName);
        if (item != null)
        {
            return (item.Data as List<IsolationConfigurationOptions<TComponentConfig>>)!;
        }
        var data = ComponentConfigUtils.GetComponentConfigs<TComponentConfig>(_serviceProvider, name, sectionName)
            .Where(option => option.Data != null!)
            .ToList();
        _componentConfigs.Add(new ComponentConfigRelationInfo()
        {
            Data = data,
            ComponentConfigType = typeof(TComponentConfig),
            SectionName = sectionName
        });
        return data;
    }

    private string GetMessage()
    {
        List<string> messages = new List<string>();
        if (_environmentContext != null)
            messages.Add($"Environment: [{_environmentContext.CurrentEnvironment ?? ""}]");
        if (_tenantContext != null)
            messages.Add($"Tenant: [{_tenantContext.CurrentTenant?.Id ?? ""}]");
        return string.Join(", ", messages);
    }
}

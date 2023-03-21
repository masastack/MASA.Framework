// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.


namespace Masa.BuildingBlocks.Isolation;

public class DefaultIsolationConfigProvider : IIsolationConfigProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMultiEnvironmentContext? _environmentContext;
    private readonly IMultiTenantContext? _tenantContext;
    private readonly ILogger<DefaultIsolationConfigProvider>? _logger;
    private readonly List<ModuleConfigRelationInfo> _data;
    private readonly List<ModuleConfigRelationInfo> _moduleConfigs;

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
        _moduleConfigs = new();
    }

    public TModuleConfig? GetModuleConfig<TModuleConfig>(string sectionName, string name) where TModuleConfig : class
    {
        var item = _data.FirstOrDefault(config => config.ModuleType == typeof(TModuleConfig) && config.SectionName == sectionName);
        if (item != null)
            return item.Data as TModuleConfig;

        Expression<Func<IsolationConfigurationOptions<TModuleConfig>, bool>> condition = option => true;

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

        var data = GetIsolationConfigurationOptions<TModuleConfig>(name, sectionName);
        TModuleConfig? moduleInfo = null;
        var modules = data
            .Where(condition.Compile())
            .OrderByDescending(option => option.Score)
            .Select(option => option.Data)
            .ToList();
        if (modules.Count >= 1)
        {
            _logger?.LogDebug("{Message}, The number of matching available configurations: {Num}",
                GetMessage(),
                modules.Count);
            moduleInfo = modules.First();
        }
        else
        {
            moduleInfo = null;
        }
        _data.Add(new ModuleConfigRelationInfo()
        {
            Data = moduleInfo,
            ModuleType = typeof(TModuleConfig),
            SectionName = sectionName
        });
        return moduleInfo;
    }

    public List<TModuleConfig> GetModuleConfigs<TModuleConfig>(string sectionName, string name) where TModuleConfig : class
    {
        var data = GetIsolationConfigurationOptions<TModuleConfig>(name, sectionName);
        var moduleConfigs = data
            .OrderByDescending(option => option.Score)
            .Select(option => option.Data)
            .ToList();
        return moduleConfigs;
    }

    private List<IsolationConfigurationOptions<TModuleConfig>> GetIsolationConfigurationOptions<TModuleConfig>(
        string name,
        string sectionName)
        where TModuleConfig : class
    {
        var item = _moduleConfigs.FirstOrDefault(config => config.ModuleType == typeof(TModuleConfig) && config.SectionName == sectionName);
        if (item != null)
        {
            return (item.Data as List<IsolationConfigurationOptions<TModuleConfig>>)!;
        }
        var data = ModuleConfigUtils.GetModuleConfigs<TModuleConfig>(_serviceProvider, name, sectionName)
            .Where(option => option.Data != null!)
            .ToList();
        _moduleConfigs.Add(new ModuleConfigRelationInfo()
        {
            Data = data,
            ModuleType = typeof(TModuleConfig),
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

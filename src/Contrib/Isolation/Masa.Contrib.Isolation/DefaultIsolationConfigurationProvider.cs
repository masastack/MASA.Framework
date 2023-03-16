// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation;

public class DefaultIsolationConfigurationProvider : IIsolationConfigurationProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMultiEnvironmentContext? _environmentContext;
    private readonly IMultiTenantContext? _tenantContext;
    private readonly ILogger<DefaultIsolationConfigurationProvider>? _logger;
    private readonly List<ModuleConfigRelationInfo> _data;

    public DefaultIsolationConfigurationProvider(
        IServiceProvider serviceProvider,
        IMultiEnvironmentContext? environmentContext = null,
        IMultiTenantContext? tenantContext = null,
        ILogger<DefaultIsolationConfigurationProvider>? logger = null)
    {
        _serviceProvider = serviceProvider;
        _environmentContext = environmentContext;
        _tenantContext = tenantContext;
        _logger = logger;
        _data = new();
    }

    public TModuleConfig? GetModuleConfig<TModuleConfig>(string sectionName) where TModuleConfig : class
    {
        var item = _data.FirstOrDefault(config => config.ModuleType == typeof(TModuleConfig) && config.SectionName == sectionName);
        if (item != null)
            return item.ModuleConfig as TModuleConfig;

        Expression<Func<IsolationConfigurationOptions<TModuleConfig>, bool>> condition = option => option.Data != null!;

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

        var data = ModuleConfigUtils.GetModules<TModuleConfig>(_serviceProvider, sectionName);
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
            ModuleConfig = moduleInfo,
            ModuleType = typeof(TModuleConfig),
            SectionName = sectionName
        });
        return moduleInfo;
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

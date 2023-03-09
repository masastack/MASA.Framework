// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Isolation;

public class DefaultIsolationConfigurationProvider : IIsolationConfigurationProvider
{
    private readonly IOptionsSnapshot<IsolationConfigurationOptions> _options;
    private readonly IMultiEnvironmentContext? _environmentContext;
    private readonly IMultiTenantContext? _tenantContext;
    private readonly ILogger<DefaultIsolationConfigurationProvider>? _logger;
    private readonly Dictionary<Type, PropertyInfo> _data;

    public DefaultIsolationConfigurationProvider(
        IOptionsSnapshot<IsolationConfigurationOptions> options,
        // IUnitOfWorkAccessor unitOfWorkAccessor,
        IMultiEnvironmentContext? environmentContext = null,
        IMultiTenantContext? tenantContext = null,
        ILogger<DefaultIsolationConfigurationProvider>? logger = null)
    {
        _options = options;
        _environmentContext = environmentContext;
        _tenantContext = tenantContext;
        _logger = logger;
        _data = new();
    }

    public bool TryGetModule<TModule>(string propertyName, [NotNullWhen(true)] out TModule? module) where TModule : class
    {
        //需要考虑获取新的配置
        // if (_unitOfWorkAccessor.CurrentDbContextOptions != null)
        //     return _unitOfWorkAccessor.CurrentDbContextOptions
        //         .ConnectionString; //todo: UnitOfWork does not currently support multi-context versions

        Expression<Func<IsolationOptions, bool>> condition = option => true;

        if (_tenantContext != null)
        {
            if (_tenantContext.CurrentTenant == null)
                _logger?.LogDebug(
                    $"Tenant resolution failed, 租户解析失败]");

            condition = condition.And(option => option.TenantId == "*" || (_tenantContext.CurrentTenant != null &&
                _tenantContext.CurrentTenant.Id.Equals(option.TenantId, StringComparison.CurrentCultureIgnoreCase)));
        }

        if (_environmentContext != null)
        {
            if (string.IsNullOrEmpty(_environmentContext.CurrentEnvironment))
            {
                _logger?.LogDebug(
                    $"Environment resolution failed, 环境解析失败]");
            }

            condition = condition.And(option
                => option.Environment == "*" ||
                option.Environment.Equals(_environmentContext.CurrentEnvironment, StringComparison.CurrentCultureIgnoreCase));
        }

        var list = _options.Value.Isolation.Where(condition.Compile()).ToList();
        if (list.Count >= 1)
        {
            var moduleInfo = list.OrderByDescending(option => option.Score).Select(option => option.Module).FirstOrDefault()!;
            if (list.Count > 1)
                _logger?.LogInformation("存在多个符合条件的配置");

            var type = moduleInfo.GetType();
            var propertyInfo = type.GetProperty(propertyName);
            MasaArgumentException.ThrowIfNull(propertyInfo, propertyName);
            if (!_data.ContainsKey(type)) _data.Add(type, propertyInfo);
            else propertyInfo = _data[type];

            module = (propertyInfo.GetValue(moduleInfo) as TModule)!;
            return true;
        }
        module = null;
        return false;
    }
}

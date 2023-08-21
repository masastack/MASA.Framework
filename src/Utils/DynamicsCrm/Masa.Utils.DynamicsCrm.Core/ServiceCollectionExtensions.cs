// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.DynamicsCrm.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCrmConfiguration(this IServiceCollection services, Action<CrmConfiguration> crmConfigurationAction)
    {
        var crmConfiguration = new CrmConfiguration();
        crmConfigurationAction.Invoke(crmConfiguration);

        services.TryAddSingleton<ICrmConfiguration>(crmConfiguration);

        return services;
    }
}

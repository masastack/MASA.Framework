// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Configuration.Tests")]
[assembly: InternalsVisibleTo("Masa.Contrib.Dispatcher.IntegrationEvents")]
[assembly: InternalsVisibleTo("Masa.Contrib.Dispatcher.IntegrationEvents.Dapr")]
[assembly: InternalsVisibleTo("Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests")]
[assembly: InternalsVisibleTo("Masa.Contrib.Dispatcher.IntegrationEvents.Tests")]
[assembly: InternalsVisibleTo("Masa.Contrib.Isolation.MultiEnvironment")]
[assembly: InternalsVisibleTo("Masa.Contrib.Isolation.MultiEnvironment.Tests")]
[assembly: InternalsVisibleTo("Masa.Contrib.Isolation.MultiTenant")]
[assembly: InternalsVisibleTo("Masa.Contrib.Isolation.MultiTenant.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

public class IsolationOptions
{
    internal string? SectionName { get; set; }

    public Type MultiTenantIdType { get; set; }

    public bool Enable => EnableMultiTenant || EnableMultiEnvironment;

    private bool _enableMultiTenant;

    private bool _enableMultiEnvironment;
    internal bool EnableMultiTenant => _enableMultiTenant;
    internal bool EnableMultiEnvironment => _enableMultiEnvironment;

    private string _multiTenantIdName;

    internal string MultiTenantIdName
    {
        get => _multiTenantIdName;
        set
        {
            _enableMultiTenant = true;
            _multiTenantIdName = value;
        }
    }

    private string _multiEnvironmentName;

    internal string MultiEnvironmentName
    {
        get => _multiEnvironmentName;
        set
        {
            _enableMultiEnvironment = true;
            _multiEnvironmentName = value;
        }
    }

    public IsolationOptions()
    {
        MultiTenantIdType = typeof(Guid);
    }
}

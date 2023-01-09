// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public class CallerOptions
{
    public List<CallerRelationOptions> Callers { get; set; } = new();

    public IServiceCollection Services { get; }

    private IEnumerable<Assembly>? _assemblies;

    public IEnumerable<Assembly> Assemblies
    {
        get => _assemblies ?? MasaApp.GetAssemblies();
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            _assemblies = value;
        }
    }

    public ServiceLifetime CallerLifetime { get; set; }

    public JsonSerializerOptions? JsonSerializerOptions { get; set; }

    public string? RequestIdKey { get; set; }

    public bool DisableAutoRegistration { get; set; } = false;

    public CallerOptions(IServiceCollection services)
    {
        Services = services;
        CallerLifetime = ServiceLifetime.Scoped;
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller.Options;

public class CallerOptions
{
    public readonly List<CallerRelationOptions> Callers = new();

    public IServiceCollection Services { get; }

    private Assembly[] _assemblies = AppDomain.CurrentDomain.GetAssemblies();

    public Assembly[] Assemblies
    {
        get => _assemblies;
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(Assemblies));

            _assemblies = value;
        }
    }

    public ServiceLifetime CallerLifetime { get; set; }

    public JsonSerializerOptions? JsonSerializerOptions { get; set; }

    public string? RequestIdKey { get; set; }

    public CallerOptions(IServiceCollection services)
    {
        Services = services;
        CallerLifetime = ServiceLifetime.Scoped;
    }
}

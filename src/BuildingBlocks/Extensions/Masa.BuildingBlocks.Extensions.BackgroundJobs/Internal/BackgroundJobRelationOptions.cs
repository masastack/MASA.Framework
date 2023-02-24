// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs;

internal class BackgroundJobRelationOptions
{
    private readonly bool _initialized;
    private readonly Dictionary<Type, TaskInvokeDelegate> _data;

    public BackgroundJobRelationOptions()
    {
        _data = new();
        _initialized = false;
    }

    public TaskInvokeDelegate GetInvokeDelegate(Type type)
    {
        if (_data.TryGetValue(type, out TaskInvokeDelegate? @delegate))
            return @delegate;

        throw new NotSupportedException(
            $"Unsupported background task type, please specify the assembly containing {type.FullName} when registering the background task");
    }

    private bool TryAdd(Type type, TaskInvokeDelegate taskInvokeDelegate)
        => _data.TryAdd(type, taskInvokeDelegate);

    public void Initialize(IEnumerable<Assembly> assemblies)
    {
        if (_initialized)
            return;

        var jobTypes = GetJobTypeList(assemblies);
        foreach (var jobType in jobTypes)
        {
            var methodInfo = jobType.GetMethod(nameof(IBackgroundJob<object>.ExecuteAsync));
            MasaArgumentException.ThrowIfNull(methodInfo);
            var @delegate = InvokeBuilder.Build(methodInfo, jobType);
            TryAdd(jobType, @delegate);
        }
    }

    private IEnumerable<Type> GetJobTypeList(IEnumerable<Assembly> assemblies)
        => assemblies
            .SelectMany(assembly => assembly.DefinedTypes)
            .Where(type =>
                type is { IsAbstract: false, IsInterface: false, IsClass: true } &&
                type.IsImplementerOfGeneric(typeof(IBackgroundJob<>)));
}

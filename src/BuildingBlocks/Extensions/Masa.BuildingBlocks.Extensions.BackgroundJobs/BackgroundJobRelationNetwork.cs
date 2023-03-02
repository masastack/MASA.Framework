// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Data.Constants;

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs;

public class BackgroundJobRelationNetwork
{
    private readonly IServiceCollection _services;
    private readonly Dictionary<Type, TaskInvokeDelegate> _delegateData;
    private readonly Dictionary<string, Type> _jobNameAndJobArgsTypeData; //Key: jobName, Value: JobArgsType
    private readonly Dictionary<string, List<Type>> _jobNameAndJobTypeData; //Key: JobName, Value: JobType List

    public BackgroundJobRelationNetwork(IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        _services = services;
        _delegateData = new();
        _jobNameAndJobArgsTypeData = new();
        _jobNameAndJobTypeData = new();
        Initialize(assemblies);
    }

    private void Initialize(IEnumerable<Assembly> assemblies)
    {
        var jobTypes = GetJobTypeList(assemblies);
        foreach (var jobType in jobTypes)
        {
            var methodInfo = jobType.GetMethod(nameof(IBackgroundJob<object>.ExecuteAsync));
            MasaArgumentException.ThrowIfNull(methodInfo);
            var taskInvokeDelegate = InvokeBuilder.Build(methodInfo, jobType);

            _delegateData.TryAdd(jobType, taskInvokeDelegate);

            var jobArgsType = methodInfo.GetParameters().Select(p => p.ParameterType).FirstOrDefault()!;
            var jobName = BackgroundJobNameAttribute.GetName(jobArgsType);

            _jobNameAndJobArgsTypeData.TryAdd(jobName, jobArgsType);

            if (_jobNameAndJobTypeData.TryGetValue(jobName, out var jobTypeList))
            {
                if (!jobTypeList.Contains(jobType))
                {
                    jobTypeList.Add(jobType);
                }
            }
            else
            {
                _jobNameAndJobTypeData.Add(jobName, new List<Type>()
                {
                    jobType
                });
            }
        }
    }

    private IEnumerable<Type> GetJobTypeList(IEnumerable<Assembly> assemblies)
        => assemblies
            .SelectMany(assembly => assembly.DefinedTypes)
            .Where(type =>
                type is { IsAbstract: false, IsInterface: false, IsClass: true } &&
                type.IsImplementerOfGeneric(typeof(IBackgroundJob<>)));

    public BackgroundJobRelationNetwork Build()
    {
        foreach (var taskInvokeDelegate in _delegateData)
        {
            _services.TryAddTransient(taskInvokeDelegate.Key);
        }
        return this;
    }

    internal TaskInvokeDelegate GetInvokeDelegate(Type type)
    {
        if (_delegateData.ContainsKey(type))
            throw new BackgroundJobException(errorCode: ExceptionErrorCode.NOT_FIND_JOB,
                type.FullName!);

        return _delegateData[type];
    }

    public List<Type> GetJobTypeList(string jobName)
    {
        if (!_jobNameAndJobTypeData.ContainsKey(jobName))
            throw new BackgroundJobException(errorCode: ExceptionErrorCode.NOT_FIND_JOB_BY_JOB_NAME,
                jobName);

        return _jobNameAndJobTypeData[jobName];
    }

    public Type GetJobArgsType(string jobName)
    {
        if (!_jobNameAndJobArgsTypeData.ContainsKey(jobName))
            throw new BackgroundJobException(errorCode: ExceptionErrorCode.NOT_FIND_JOB_ARGS_BY_JOB_NAME,
                jobName);

        return _jobNameAndJobArgsTypeData[jobName];
    }
}

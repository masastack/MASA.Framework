// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Log.Elasticseach;

public static class ServiceExtenistion
{
    public static IServiceCollection AddElasticClientLog(this IServiceCollection services, string[] nodes, string indexName, IEnumerable<ElasticseacherMappingResponseDto>? mappings = null)
    {
        ElasticConst.Log.Init(indexName, mappings!, true);
        return AddElasticsearch(services, nodes, ElasticConst.LOG_CALLER_CLIENT_NAME, true).AddSingleton<ILogService, LogService>();
    }

    public static IServiceCollection AddElasticClientLog(this IServiceCollection services, Action<ElasticsearchOptions> elasearchConnectionAction, Action<MasaHttpClientBuilder> callerAction, string indexName, IEnumerable<ElasticseacherMappingResponseDto>? mappings = null)
    {
        ElasticConst.Log.Init(indexName, mappings!, true);
        return AddElasticsearch(services, elasearchConnectionAction, callerAction, ElasticConst.LOG_CALLER_CLIENT_NAME).AddSingleton<ILogService, LogService>();
    }

    public static IServiceCollection AddElasticClientTrace(this IServiceCollection services, string[] nodes, string indexName, IEnumerable<ElasticseacherMappingResponseDto>? mappings = null)
    {
        ElasticConst.Trace.Init(indexName, mappings!, true);
        return AddElasticsearch(services, nodes, ElasticConst.TRACE_CALLER_CLIENT_NAME).AddSingleton<ITraceService, TraceService>();
    }

    public static IServiceCollection AddElasticClientTrace(this IServiceCollection services, Action<ElasticsearchOptions> elasearchConnectionAction, string indexName, IEnumerable<ElasticseacherMappingResponseDto>? mappings = null)
    {
        ElasticConst.Trace.Init(indexName, mappings!, true);
        return AddElasticsearch(services, elasearchConnectionAction, default!, ElasticConst.TRACE_CALLER_CLIENT_NAME).AddSingleton<ITraceService, TraceService>();
    }

    public static IServiceCollection AddElasticClientLogAndTrace(this IServiceCollection services, string[] nodes, string logIndexName, string traceIndexName, IEnumerable<ElasticseacherMappingResponseDto>? logMappings = null, IEnumerable<ElasticseacherMappingResponseDto>? traceMappings = null)
    {
        ElasticConst.Log.Init(logIndexName, logMappings!, false);
        ElasticConst.Trace.Init(traceIndexName, traceMappings!, false);
        return AddElasticsearch(services, nodes, ElasticConst.DEFAULT_CALLER_CLIENT_NAME)
            .AddSingleton<ILogService, LogService>()
            .AddSingleton<ITraceService, TraceService>();
    }

    public static IServiceCollection AddElasticClientLogAndTrace(this IServiceCollection services, Action<ElasticsearchOptions> elasearchConnectionAction, Action<MasaHttpClientBuilder> callerAction, string logIndexName, string traceIndexName, IEnumerable<ElasticseacherMappingResponseDto>? logMappings = null, IEnumerable<ElasticseacherMappingResponseDto>? traceMappings = null)
    {
        ElasticConst.Log.Init(logIndexName, logMappings!);
        ElasticConst.Trace.Init(traceIndexName, traceMappings!);
        return AddElasticsearch(services, elasearchConnectionAction, callerAction, ElasticConst.DEFAULT_CALLER_CLIENT_NAME)
            .AddSingleton<ILogService, LogService>()
            .AddSingleton<ITraceService, TraceService>();
    }

    internal static IElasticClient CreateElasticClient(this IElasticsearchFactory elasticsearchFactory, bool isLog = true)
    {
        if (isLog)
            return elasticsearchFactory.CreateElasticClient(ElasticConst.Log.IsIndependent ? ElasticConst.LOG_CALLER_CLIENT_NAME : ElasticConst.DEFAULT_CALLER_CLIENT_NAME);
        else
            return elasticsearchFactory.CreateElasticClient(ElasticConst.Trace.IsIndependent ? ElasticConst.TRACE_CALLER_CLIENT_NAME : ElasticConst.DEFAULT_CALLER_CLIENT_NAME);
    }

    internal static ICaller Create(this ICallerFactory callerFactory, bool isLog)
    {
        if (isLog)
            return callerFactory.Create(ElasticConst.Log.IsIndependent ? ElasticConst.LOG_CALLER_CLIENT_NAME : ElasticConst.DEFAULT_CALLER_CLIENT_NAME);
        else
            return callerFactory.Create(ElasticConst.Trace.IsIndependent ? ElasticConst.TRACE_CALLER_CLIENT_NAME : ElasticConst.DEFAULT_CALLER_CLIENT_NAME);
    }

    private static IServiceCollection AddElasticsearch(IServiceCollection services, string[] nodes, string name, bool hasCaller = false)
    {
        services.AddElasticsearch(name, nodes);
        if (hasCaller)
            services.AddCaller(option =>
            {
                option.DisableAutoRegistration = true;
                option.UseHttpClient(name, builder =>
                {
                    builder.BaseAddress = nodes[0];
                });
            });
        return services;
    }

    private static IServiceCollection AddElasticsearch(IServiceCollection services, Action<ElasticsearchOptions> elasearchConnectionAction, Action<MasaHttpClientBuilder>? callerAction, string name)
    {
        services.AddElasticsearch(name, elasearchConnectionAction);
        if (callerAction != null)
            services.AddCaller(option =>
            {
                option.DisableAutoRegistration = true;
                if (callerAction != null)
                    option.UseHttpClient(name, callerAction);
            });
        return services;
    }
}

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticseach;

public static class ServiceExtenistion
{
    public static IServiceCollection AddElasticClientLog(this IServiceCollection services, string[] nodes, string indexName)
    {
        ElasticConst.InitLog(indexName, true);
        AddElasticsearch(services, nodes, ElasticConst.LOG_CALLER_CLIENT_NAME).AddSingleton<ILogService, LogService>();
        ElasticConst.Log.Mappings = GetLazyMapping(services, ElasticConst.LOG_CALLER_CLIENT_NAME, indexName);
        return services;
    }

    public static IServiceCollection AddElasticClientLog(this IServiceCollection services, Action<ElasticsearchOptions> elasearchConnectionAction, Action<MasaHttpClientBuilder> callerAction, string indexName)
    {
        ElasticConst.InitLog(indexName, true);
        AddElasticsearch(services, elasearchConnectionAction, callerAction, ElasticConst.LOG_CALLER_CLIENT_NAME).AddSingleton<ILogService, LogService>();
        ElasticConst.Log.Mappings = GetLazyMapping(services, ElasticConst.LOG_CALLER_CLIENT_NAME, indexName);
        return services;
    }

    public static IServiceCollection AddElasticClientTrace(this IServiceCollection services, string[] nodes, string indexName)
    {
        ElasticConst.InitTrace(indexName, true);
        AddElasticsearch(services, nodes, ElasticConst.TRACE_CALLER_CLIENT_NAME).AddSingleton<ITraceService, TraceService>();
        ElasticConst.Trace.Mappings = GetLazyMapping(services, ElasticConst.TRACE_CALLER_CLIENT_NAME, indexName);
        return services;
    }

    public static IServiceCollection AddElasticClientTrace(this IServiceCollection services, Action<ElasticsearchOptions> elasearchConnectionAction, Action<MasaHttpClientBuilder> callerAction, string indexName)
    {
        ElasticConst.InitTrace(indexName, true);
        AddElasticsearch(services, elasearchConnectionAction, callerAction, ElasticConst.TRACE_CALLER_CLIENT_NAME).AddSingleton<ITraceService, TraceService>();
        ElasticConst.Trace.Mappings = GetLazyMapping(services, ElasticConst.TRACE_CALLER_CLIENT_NAME, indexName);
        return services;
    }

    public static IServiceCollection AddElasticClientLogAndTrace(this IServiceCollection services, string[] nodes, string logIndexName, string traceIndexName)
    {
        ElasticConst.InitLog(logIndexName, false);
        ElasticConst.InitTrace(traceIndexName, false);
        AddElasticsearch(services, nodes, ElasticConst.DEFAULT_CALLER_CLIENT_NAME)
            .AddSingleton<ILogService, LogService>()
            .AddSingleton<ITraceService, TraceService>();
        ElasticConst.Log.Mappings = GetLazyMapping(services, ElasticConst.DEFAULT_CALLER_CLIENT_NAME, logIndexName);
        ElasticConst.Trace.Mappings = GetLazyMapping(services, ElasticConst.DEFAULT_CALLER_CLIENT_NAME, traceIndexName);
        return services;
    }

    public static IServiceCollection AddElasticClientLogAndTrace(this IServiceCollection services, Action<ElasticsearchOptions> elasearchConnectionAction, Action<MasaHttpClientBuilder> callerAction, string logIndexName, string traceIndexName)
    {
        ElasticConst.InitLog(logIndexName);
        ElasticConst.InitTrace(traceIndexName);
        AddElasticsearch(services, elasearchConnectionAction, callerAction, ElasticConst.DEFAULT_CALLER_CLIENT_NAME)
            .AddSingleton<ILogService, LogService>()
            .AddSingleton<ITraceService, TraceService>();
        ElasticConst.Log.Mappings = GetLazyMapping(services, ElasticConst.DEFAULT_CALLER_CLIENT_NAME, logIndexName);
        ElasticConst.Trace.Mappings = GetLazyMapping(services, ElasticConst.DEFAULT_CALLER_CLIENT_NAME, traceIndexName);
        return services;
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

    private static IServiceCollection AddElasticsearch(IServiceCollection services, string[] nodes, string name)
    {
        return services.AddElasticsearch(name, options =>
        {
            options.UseNodes(nodes).UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));

        })
            .AddCaller(option =>
            {
                option.DisableAutoRegistration = true;
                option.UseHttpClient(name, builder =>
                {
                    builder.BaseAddress = nodes[0];
                });
            });
    }

    private static IServiceCollection AddElasticsearch(IServiceCollection services, Action<ElasticsearchOptions> elasearchConnectionAction, Action<MasaHttpClientBuilder> callerAction, string name)
    {
        ArgumentNullException.ThrowIfNull(callerAction);

        return services.AddElasticsearch(name, elasearchConnectionAction)
         .AddCaller(option =>
             {
                 option.DisableAutoRegistration = true;
                 option.UseHttpClient(name, callerAction);
             });
    }

    private static Lazy<ElasticseacherMappingResponseDto[]> GetLazyMapping(IServiceCollection services, string callerName, string indexName)
    {
        return new Lazy<ElasticseacherMappingResponseDto[]>(() =>
        {
            var callerFactory = services.BuildServiceProvider().GetRequiredService<ICallerFactory>();
            var caller = callerFactory.Create(callerName);
            var result = caller.GetMappingAsync(indexName).Result;
            return result.Select(item => (ElasticseacherMappingResponseDto)item).ToArray();
        });
    }
}

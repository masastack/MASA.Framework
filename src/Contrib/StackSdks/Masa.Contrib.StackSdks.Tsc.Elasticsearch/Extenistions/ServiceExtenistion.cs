// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceExtenistion
{
    public static IServiceCollection AddElasticClientLog(this IServiceCollection services, string[] nodes, string indexName)
    {
        ElasticConstant.InitLog(indexName, true);
        if (services.BuildServiceProvider().GetService<ILogService>() == null)
            AddElasticsearch(services, nodes, ElasticConstant.LOG_CALLER_CLIENT_NAME).AddScoped<ILogService, LogService>();
        ElasticConstant.Log.Mappings = GetLazyMapping(services, ElasticConstant.LOG_CALLER_CLIENT_NAME, indexName);
        return services;
    }

    public static IServiceCollection AddElasticClientLog(this IServiceCollection services,
        Action<ElasticsearchOptions> elasearchConnectionAction, Action<MasaHttpClient> callerAction, string indexName)
    {
        ElasticConstant.InitLog(indexName, true);
        if (services.BuildServiceProvider().GetService<ILogService>() == null)
            AddElasticsearch(services, elasearchConnectionAction, callerAction, ElasticConstant.LOG_CALLER_CLIENT_NAME)
                .AddScoped<ILogService, LogService>();
        ElasticConstant.Log.Mappings = GetLazyMapping(services, ElasticConstant.LOG_CALLER_CLIENT_NAME, indexName);
        return services;
    }

    public static IServiceCollection AddElasticClientTrace(this IServiceCollection services, string[] nodes, string indexName)
    {
        ElasticConstant.InitTrace(indexName, true);
        if (services.BuildServiceProvider().GetService<ITraceService>() == null)
            AddElasticsearch(services, nodes, ElasticConstant.TRACE_CALLER_CLIENT_NAME).AddScoped<ITraceService, TraceService>();
        ElasticConstant.Trace.Mappings = GetLazyMapping(services, ElasticConstant.TRACE_CALLER_CLIENT_NAME, indexName);
        return services;
    }

    public static IServiceCollection AddElasticClientTrace(this IServiceCollection services,
        Action<ElasticsearchOptions> elasearchConnectionAction, Action<MasaHttpClient> callerAction, string indexName)
    {
        ElasticConstant.InitTrace(indexName, true);
        if (services.BuildServiceProvider().GetService<ITraceService>() == null)
            AddElasticsearch(services, elasearchConnectionAction, callerAction, ElasticConstant.TRACE_CALLER_CLIENT_NAME)
                .AddScoped<ITraceService, TraceService>();
        ElasticConstant.Trace.Mappings = GetLazyMapping(services, ElasticConstant.TRACE_CALLER_CLIENT_NAME, indexName);
        return services;
    }

    public static IServiceCollection AddElasticClientLogAndTrace(this IServiceCollection services, string[] nodes, string logIndexName,
        string traceIndexName)
    {
        ElasticConstant.InitLog(logIndexName);
        ElasticConstant.InitTrace(traceIndexName);
        if (services.BuildServiceProvider().GetService<ILogService>() == null || services.BuildServiceProvider().GetService<ITraceService>() == null)
            AddElasticsearch(services, nodes, ElasticConstant.DEFAULT_CALLER_CLIENT_NAME)
                .AddScoped<ILogService, LogService>()
                .AddScoped<ITraceService, TraceService>();
        ElasticConstant.Log.Mappings = GetLazyMapping(services, ElasticConstant.DEFAULT_CALLER_CLIENT_NAME, logIndexName);
        ElasticConstant.Trace.Mappings = GetLazyMapping(services, ElasticConstant.DEFAULT_CALLER_CLIENT_NAME, traceIndexName);
        return services;
    }

    public static IServiceCollection AddElasticClientLogAndTrace(this IServiceCollection services,
        Action<ElasticsearchOptions> elasearchConnectionAction, Action<MasaHttpClient> callerAction, string logIndexName,
        string traceIndexName)
    {
        ElasticConstant.InitLog(logIndexName);
        ElasticConstant.InitTrace(traceIndexName);
        if (services.BuildServiceProvider().GetService<ILogService>() == null || services.BuildServiceProvider().GetService<ITraceService>() == null)
            AddElasticsearch(services, elasearchConnectionAction, callerAction, ElasticConstant.DEFAULT_CALLER_CLIENT_NAME)
                .AddScoped<ILogService, LogService>()
                .AddScoped<ITraceService, TraceService>();
        ElasticConstant.Log.Mappings = GetLazyMapping(services, ElasticConstant.DEFAULT_CALLER_CLIENT_NAME, logIndexName);
        ElasticConstant.Trace.Mappings = GetLazyMapping(services, ElasticConstant.DEFAULT_CALLER_CLIENT_NAME, traceIndexName);
        return services;
    }

    private static IServiceCollection AddElasticsearch(IServiceCollection services, string[] nodes, string name)
    {
        return services.AddElasticsearch(name, options =>
            {
                options.UseNodes(nodes).UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));

            })
            .AddCaller(name, callerBuilder =>
            {
                callerBuilder.UseHttpClient(builder =>
                {
                    builder.BaseAddress = nodes[0];
                }).UseAuthentication();
            });
    }

    private static IServiceCollection AddElasticsearch(IServiceCollection services,
        Action<ElasticsearchOptions> elasticsearchConnectionAction,
        Action<MasaHttpClient> callerAction, string name)
    {
        ArgumentNullException.ThrowIfNull(callerAction);
        var factory = services.BuildServiceProvider().GetService<IElasticClientFactory>();
        var callerFactory = services.BuildServiceProvider().GetService<ICallerFactory>();

        if (factory == null || factory.Create(name) == null || callerFactory == null || callerFactory.Create(name) == null)
            services.AddElasticsearch(name, elasticsearchConnectionAction)
               .AddCaller(name, option => option.UseHttpClient(callerAction).UseAuthentication());
        return services;
    }

    internal static IElasticClient CreateElasticClient(this IElasticClientFactory elasticsearchFactory, bool isLog)
    {
        if (isLog)
            return elasticsearchFactory.Create(ElasticConstant.Log.IsIndependent ? ElasticConstant.LOG_CALLER_CLIENT_NAME :
                ElasticConstant.DEFAULT_CALLER_CLIENT_NAME);
        else
            return elasticsearchFactory.Create(ElasticConstant.Trace.IsIndependent ? ElasticConstant.TRACE_CALLER_CLIENT_NAME :
                ElasticConstant.DEFAULT_CALLER_CLIENT_NAME);
    }

    internal static ICaller Create(this ICallerFactory callerFactory, bool isLog)
    {
        if (isLog)
            return callerFactory.Create(ElasticConstant.Log.IsIndependent ? ElasticConstant.LOG_CALLER_CLIENT_NAME :
                ElasticConstant.DEFAULT_CALLER_CLIENT_NAME);
        else
            return callerFactory.Create(ElasticConstant.Trace.IsIndependent ? ElasticConstant.TRACE_CALLER_CLIENT_NAME :
                ElasticConstant.DEFAULT_CALLER_CLIENT_NAME);
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

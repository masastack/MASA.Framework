// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticseach.Tests.Extensions;

[TestClass]
public class ServiceExtenistionTests
{
    [TestInitialize]
    public void TestInitialize()
    {
        ServiceCollection services = new();
        services.Clear();
    }

    [TestMethod]
    public void AddLogNodesTest()
    {
        ServiceCollection services = new();
        services.AddElasticClientLog(new string[] { StaticConfig.HOST }, StaticConfig.LOG_INDEX_NAME);
        Assert.IsNotNull(ElasticConstant.Log);
        Assert.AreEqual(ElasticConstant.Log.IndexName, StaticConfig.LOG_INDEX_NAME);

        var factory = services.BuildServiceProvider().GetRequiredService<IElasticsearchFactory>();
        Assert.IsNotNull(factory);
        Assert.IsNotNull(factory.CreateElasticClient(ElasticConstant.LOG_CALLER_CLIENT_NAME));
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConstant.TRACE_CALLER_CLIENT_NAME));
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConstant.DEFAULT_CALLER_CLIENT_NAME));
    }

    [TestMethod]
    public void AddLogOptionsTest()
    {
        ServiceCollection services = new();
        services.AddElasticClientLog(options =>
        {
            options.UseNodes(new string[] { StaticConfig.HOST })
            .UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
        }, caller =>
        {
            caller.BaseAddress = StaticConfig.HOST;
        }, StaticConfig.LOG_INDEX_NAME);

        Assert.IsNotNull(ElasticConstant.Log);
        Assert.AreEqual(ElasticConstant.Log.IndexName, StaticConfig.LOG_INDEX_NAME);

        var factory = services.BuildServiceProvider().GetRequiredService<IElasticsearchFactory>();
        Assert.IsNotNull(factory);
        Assert.IsNotNull(factory.CreateElasticClient(ElasticConstant.LOG_CALLER_CLIENT_NAME));
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConstant.TRACE_CALLER_CLIENT_NAME));
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConstant.DEFAULT_CALLER_CLIENT_NAME));
    }

    [TestMethod]
    public void AddTraceNodesTest()
    {
        ServiceCollection services = new();
        services.AddElasticClientTrace(new string[] { StaticConfig.HOST }, StaticConfig.TRACE_INDEX_NAME);
        Assert.IsNotNull(ElasticConstant.Trace);
        Assert.AreEqual(ElasticConstant.Trace.IndexName, StaticConfig.TRACE_INDEX_NAME);

        var factory = services.BuildServiceProvider().GetRequiredService<IElasticsearchFactory>();
        Assert.IsNotNull(factory);
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConstant.LOG_CALLER_CLIENT_NAME));
        Assert.IsNotNull(factory.CreateElasticClient(ElasticConstant.TRACE_CALLER_CLIENT_NAME));
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConstant.DEFAULT_CALLER_CLIENT_NAME));
    }

    [TestMethod]
    public void AddTraceOptionsTest()
    {
        ServiceCollection services = new();
        services.AddElasticClientTrace(options =>
        {
            options.UseNodes(new string[] { StaticConfig.HOST }).
            UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
        }, caller =>
        {
            caller.BaseAddress = StaticConfig.HOST;
        }, StaticConfig.TRACE_INDEX_NAME);

        Assert.IsNotNull(ElasticConstant.Trace);
        Assert.AreEqual(ElasticConstant.Trace.IndexName, StaticConfig.TRACE_INDEX_NAME);

        var factory = services.BuildServiceProvider().GetRequiredService<IElasticsearchFactory>();
        Assert.IsNotNull(factory);
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConstant.LOG_CALLER_CLIENT_NAME));
        Assert.IsNotNull(factory.CreateElasticClient(ElasticConstant.TRACE_CALLER_CLIENT_NAME));
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConstant.DEFAULT_CALLER_CLIENT_NAME));
    }

    [TestMethod]
    public void AddLogTraceNodesTest()
    {
        ServiceCollection services = new();
        services.AddElasticClientLogAndTrace(new string[] { StaticConfig.HOST }, StaticConfig.LOG_INDEX_NAME, StaticConfig.TRACE_INDEX_NAME);
        Assert.IsNotNull(ElasticConstant.Trace);
        Assert.AreEqual(ElasticConstant.Trace.IndexName, StaticConfig.TRACE_INDEX_NAME);
        Assert.IsNotNull(ElasticConstant.Log);
        Assert.AreEqual(ElasticConstant.Log.IndexName, StaticConfig.LOG_INDEX_NAME);

        var factory = services.BuildServiceProvider().GetRequiredService<IElasticsearchFactory>();
        Assert.IsNotNull(factory);
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConstant.LOG_CALLER_CLIENT_NAME));
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConstant.TRACE_CALLER_CLIENT_NAME));
        Assert.IsNotNull(factory.CreateElasticClient(ElasticConstant.DEFAULT_CALLER_CLIENT_NAME));
    }

    [TestMethod]
    public void AddLogTraceOptionsTest()
    {
        ServiceCollection services = new();
        services.AddElasticClientLogAndTrace(options =>
        {
            options.UseNodes(new string[] { StaticConfig.HOST }).
            UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
        }, caller =>
        {
            caller.BaseAddress = StaticConfig.HOST;
        },
        StaticConfig.LOG_INDEX_NAME,
        StaticConfig.TRACE_INDEX_NAME);

        Assert.IsNotNull(ElasticConstant.Trace);
        Assert.AreEqual(ElasticConstant.Trace.IndexName, StaticConfig.TRACE_INDEX_NAME);
        Assert.IsNotNull(ElasticConstant.Log);
        Assert.AreEqual(ElasticConstant.Log.IndexName, StaticConfig.LOG_INDEX_NAME);

        var factory = services.BuildServiceProvider().GetRequiredService<IElasticsearchFactory>();
        Assert.IsNotNull(factory);
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConstant.LOG_CALLER_CLIENT_NAME));
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConstant.TRACE_CALLER_CLIENT_NAME));
        Assert.IsNotNull(factory.CreateElasticClient(ElasticConstant.DEFAULT_CALLER_CLIENT_NAME));
    }
}

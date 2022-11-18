// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Utils.Data.Elasticsearch;

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
        Assert.IsNotNull(ElasticConst.Log);
        Assert.AreEqual(ElasticConst.Log.IndexName, StaticConfig.LOG_INDEX_NAME);

        var factory = services.BuildServiceProvider().GetRequiredService<IElasticsearchFactory>();
        Assert.IsNotNull(factory);
        Assert.IsNotNull(factory.CreateElasticClient(ElasticConst.LOG_CALLER_CLIENT_NAME));
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConst.TRACE_CALLER_CLIENT_NAME));
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConst.DEFAULT_CALLER_CLIENT_NAME));
    }

    [TestMethod]
    public void AddLogOptionsTest()
    {
        ServiceCollection services = new();
        services.AddElasticClientLog(options =>
        {
            options.UseNodes(new string[] { StaticConfig.HOST });
        }, caller =>
        {
            caller.BaseAddress = StaticConfig.HOST;
        }, StaticConfig.LOG_INDEX_NAME);

        Assert.IsNotNull(ElasticConst.Log);
        Assert.AreEqual(ElasticConst.Log.IndexName, StaticConfig.LOG_INDEX_NAME);

        var factory = services.BuildServiceProvider().GetRequiredService<IElasticsearchFactory>();
        Assert.IsNotNull(factory);
        Assert.IsNotNull(factory.CreateElasticClient(ElasticConst.LOG_CALLER_CLIENT_NAME));
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConst.TRACE_CALLER_CLIENT_NAME));
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConst.DEFAULT_CALLER_CLIENT_NAME));
    }

    [TestMethod]
    public void AddTraceNodesTest()
    {
        ServiceCollection services = new();
        services.AddElasticClientTrace(new string[] { StaticConfig.HOST }, StaticConfig.TRACE_INDEX_NAME);
        Assert.IsNotNull(ElasticConst.Trace);
        Assert.AreEqual(ElasticConst.Trace.IndexName, StaticConfig.TRACE_INDEX_NAME);

        var factory = services.BuildServiceProvider().GetRequiredService<IElasticsearchFactory>();
        Assert.IsNotNull(factory);
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConst.LOG_CALLER_CLIENT_NAME));
        Assert.IsNotNull(factory.CreateElasticClient(ElasticConst.TRACE_CALLER_CLIENT_NAME));
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConst.DEFAULT_CALLER_CLIENT_NAME));
    }

    [TestMethod]
    public void AddTraceOptionsTest()
    {
        ServiceCollection services = new();
        services.AddElasticClientTrace(options =>
        {
            options.UseNodes(new string[] { StaticConfig.HOST });
        }, caller =>
        {
            caller.BaseAddress = StaticConfig.HOST;
        }, StaticConfig.TRACE_INDEX_NAME);

        Assert.IsNotNull(ElasticConst.Trace);
        Assert.AreEqual(ElasticConst.Trace.IndexName, StaticConfig.TRACE_INDEX_NAME);

        var factory = services.BuildServiceProvider().GetRequiredService<IElasticsearchFactory>();
        Assert.IsNotNull(factory);
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConst.LOG_CALLER_CLIENT_NAME));
        Assert.IsNotNull(factory.CreateElasticClient(ElasticConst.TRACE_CALLER_CLIENT_NAME));
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConst.DEFAULT_CALLER_CLIENT_NAME));
    }

    [TestMethod]
    public void AddLogTraceNodesTest()
    {
        ServiceCollection services = new();
        services.AddElasticClientLogAndTrace(new string[] { StaticConfig.HOST }, StaticConfig.LOG_INDEX_NAME, StaticConfig.TRACE_INDEX_NAME);
        Assert.IsNotNull(ElasticConst.Trace);
        Assert.AreEqual(ElasticConst.Trace.IndexName, StaticConfig.TRACE_INDEX_NAME);
        Assert.IsNotNull(ElasticConst.Log);
        Assert.AreEqual(ElasticConst.Log.IndexName, StaticConfig.LOG_INDEX_NAME);

        var factory = services.BuildServiceProvider().GetRequiredService<IElasticsearchFactory>();
        Assert.IsNotNull(factory);
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConst.LOG_CALLER_CLIENT_NAME));
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConst.TRACE_CALLER_CLIENT_NAME));
        Assert.IsNotNull(factory.CreateElasticClient(ElasticConst.DEFAULT_CALLER_CLIENT_NAME));
    }

    [TestMethod]
    public void AddLogTraceOptionsTest()
    {
        ServiceCollection services = new();
        services.AddElasticClientLogAndTrace(options =>
        {
            options.UseNodes(new string[] { StaticConfig.HOST });
        }, caller =>
        {
            caller.BaseAddress = StaticConfig.HOST;
        },
        StaticConfig.LOG_INDEX_NAME,
        StaticConfig.TRACE_INDEX_NAME);

        Assert.IsNotNull(ElasticConst.Trace);
        Assert.AreEqual(ElasticConst.Trace.IndexName, StaticConfig.TRACE_INDEX_NAME);
        Assert.IsNotNull(ElasticConst.Log);
        Assert.AreEqual(ElasticConst.Log.IndexName, StaticConfig.LOG_INDEX_NAME);

        var factory = services.BuildServiceProvider().GetRequiredService<IElasticsearchFactory>();
        Assert.IsNotNull(factory);
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConst.LOG_CALLER_CLIENT_NAME));
        Assert.ThrowsException<NotSupportedException>(() => factory.CreateElasticClient(ElasticConst.TRACE_CALLER_CLIENT_NAME));
        Assert.IsNotNull(factory.CreateElasticClient(ElasticConst.DEFAULT_CALLER_CLIENT_NAME));
    }
}

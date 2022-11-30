// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticsearch.Tests.Converts;

[TestClass]
public class TraceConverterTests
{
    [TestMethod]
    public void DeserializeTest()
    {
        var str = StaticConfig.GetJson("Convert:traceJson");
        JsonSerializerOptions options = new();
        options.Converters.Add(new TraceResponseDtoConverter());

        var trace = JsonSerializer.Deserialize<TraceResponseDto>(str, options);
        Assert.IsNotNull(trace);
        Assert.IsNotNull(trace.Resource);
        Assert.IsNotNull(trace.Attributes);
        Assert.AreEqual(0, trace.TraceStatus);
        Assert.AreEqual("277df6d0204f09fa63ff4ab896673455", trace.TraceId);
        Assert.AreEqual("1c495129b86de343", trace.SpanId);
        Assert.IsNotNull(trace.Kind);

        var isHttp = trace.TryParseHttp(out var httpResult);
        Assert.IsTrue(isHttp);
        Assert.IsNotNull(httpResult);
        Assert.IsNotNull(httpResult.Method);
    }
}

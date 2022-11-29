// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticsearch.Tests.Converts;

[TestClass]
public class LogConverterTests
{
    [TestMethod]
    public void DeserializeTest()
    {
        var str = StaticConfig.GetJson("Convert:logJson");
        JsonSerializerOptions options = new();
        options.Converters.Add(new LogResponseDtoConverter());

        var log = JsonSerializer.Deserialize<LogResponseDto>(str, options);
        Assert.IsNotNull(log);
        Assert.IsNotNull(log.Resource);
        Assert.IsNotNull(log.Attributes);
        Assert.AreEqual(1, log.TraceFlags);
        Assert.AreEqual(9, log.SeverityNumber);
        Assert.AreEqual("Information", log.SeverityText);
        Assert.AreEqual("277df6d0204f09fa63ff4ab896673455", log.TraceId);
        Assert.AreEqual("1c495129b86de343", log.SpanId);
        Assert.IsNotNull(log.Body);
    }
}

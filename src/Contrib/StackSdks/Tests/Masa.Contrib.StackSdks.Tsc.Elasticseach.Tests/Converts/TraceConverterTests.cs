// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticseach.Tests.Converts;

[TestClass]
public class TraceConverterTests
{
    private readonly string strJson = "{\"@timestamp\":\"2022-11-15T12:12:10.938123800Z\",\"Attributes.host.name\":\"SSKJ016\",\"Attributes.http.client_ip\":\"::1\",\"Attributes.http.flavor\":\"HTTP/2\",\"Attributes.http.host\":\"localhost:18012\",\"Attributes.http.method\":\"GET\",\"Attributes.http.response_content_length\":0,\"Attributes.http.scheme\":\"https\",\"Attributes.http.status_code\":302,\"Attributes.http.target\":\"/\",\"Attributes.http.url\":\"https://localhost:18012/\",\"Attributes.http.user_agent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36\",\"EndTimestamp\":\"2022-11-15T12:12:11.323348000Z\",\"Kind\":\"SPAN_KIND_SERVER\",\"Link\":\"[]\",\"Name\":\"/\",\"Resource.service.instance.id\":\"9eaf1452-8b12-4368-928c-48f871c250fe\",\"Resource.service.name\":\"masa-tsc-web-admin\",\"Resource.service.namespace\":\"Development\",\"Resource.service.version\":\"0.1.0\",\"Resource.telemetry.sdk.language\":\"dotnet\",\"Resource.telemetry.sdk.name\":\"opentelemetry\",\"Resource.telemetry.sdk.version\":\"1.3.0.519\",\"SpanId\":\"1c495129b86de343\",\"TraceId\":\"277df6d0204f09fa63ff4ab896673455\",\"TraceStatus\":0}";

    [TestMethod]
    public void DeserializeTest()
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.Converters.Add(new TraceResponseDtoConverter());

        var trace = JsonSerializer.Deserialize<TraceResponseDto>(strJson, options);
        Assert.IsNotNull(trace);
        Assert.IsNotNull(trace.Resource);
        Assert.IsNotNull(trace.Attributes);
        Assert.AreEqual(0, trace.TraceStatus);
        Assert.AreEqual("277df6d0204f09fa63ff4ab896673455", trace.TraceId);
        Assert.AreEqual("1c495129b86de343", trace.SpanId);
        Assert.IsNotNull(trace.Kind);

        var isHttp=trace.IsHttp(out var httpResult);
        Assert.IsTrue(isHttp);
        Assert.IsNotNull(httpResult);
        Assert.IsNotNull(httpResult.Method);
    }
}

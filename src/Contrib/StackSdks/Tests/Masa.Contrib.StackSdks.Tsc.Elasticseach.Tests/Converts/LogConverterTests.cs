// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticseach.Tests.Converts;

[TestClass]
public class LogConverterTests
{
    private readonly string strJson = "{\"@timestamp\":\"2022-11-15T12:12:11.028116800Z\",\"Attributes.Id\":2,\"Attributes.Name\":\"UserAuthorizationFailed\",\"Attributes.Reason\":\"These requirements were not met:\\r\\nDenyAnonymousAuthorizationRequirement: Requires an authenticated user.\",\"Attributes.[Scope.0]:ParentId\":\"0000000000000000\",\"Attributes.[Scope.0]:SpanId\":\"1c495129b86de343\",\"Attributes.[Scope.0]:TraceId\":\"277df6d0204f09fa63ff4ab896673455\",\"Attributes.[Scope.1]:ConnectionId\":\"0HMM705VTO6BQ\",\"Attributes.[Scope.2]:RequestId\":\"0HMM705VTO6BQ:00000001\",\"Attributes.[Scope.2]:RequestPath\":\"/\",\"Attributes.dotnet.ilogger.category\":\"Microsoft.AspNetCore.Authorization.DefaultAuthorizationService\",\"Attributes.{OriginalFormat}\":\"Authorization failed. {Reason}\",\"Body\":\"Authorization failed. These requirements were not met:\\r\\nDenyAnonymousAuthorizationRequirement: Requires an authenticated user.\",\"Resource.service.instance.id\":\"9eaf1452-8b12-4368-928c-48f871c250fe\",\"Resource.service.name\":\"masa-tsc-web-admin\",\"Resource.service.namespace\":\"Development\",\"Resource.service.version\":\"0.1.0\",\"Resource.telemetry.sdk.language\":\"dotnet\",\"Resource.telemetry.sdk.name\":\"opentelemetry\",\"Resource.telemetry.sdk.version\":\"1.3.0.519\",\"SeverityNumber\":9,\"SeverityText\":\"Information\",\"SpanId\":\"1c495129b86de343\",\"TraceFlags\":1,\"TraceId\":\"277df6d0204f09fa63ff4ab896673455\"}";

    [TestMethod]
    public void DeserializeTest()
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.Converters.Add(new LogResponseDtoConverter());

        var log = JsonSerializer.Deserialize<LogResponseDto>(strJson, options);
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

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.OpenTelemetry.Tests.Trace;

[TestClass]
public class ActivityTest
{
    [TestInitialize]
    public void Initialize()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    [TestMethod]
    public void HttpRequestMessageAddTagsTest()
    {
        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Post
        };
        var body = "{\"name\":\"张三\"}";
        request.Content = new StringContent(body, Encoding.GetEncoding("gbk"), "application/json");
        request.RequestUri = new Uri("http://localhost");

        var activity = new Activity("tets");
        activity.AddMasaSupplement(request);
        Assert.AreEqual(body, activity.GetTagItem(OpenTelemetryAttributeName.Http.REQUEST_CONTENT_BODY) as string);
    }

    [TestMethod]
    public void HttpResponseMessageAddTagsTest()
    {
        HttpResponseMessage response = new()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("OK")
        };

        var activity = new Activity("tets");
        activity.AddMasaSupplement(response);
        Assert.IsNotNull(activity);
    }

    [TestMethod]
    public void HttpRequestAddTagsTest()
    {
        Mock<HttpRequest> mock = new();
        mock.Setup(request => request.Method).Returns("post");
        mock.Setup(request => request.ContentType).Returns(" application/json; charset=gbk");
        mock.Setup(request => request.Host).Returns(new HostString("http://localhost"));
        mock.Setup(request => request.Protocol).Returns("http1.1");
        mock.Setup(request => request.Scheme).Returns("http");
        var httpContext = new Mock<HttpContext>();
        mock.Setup(request => request.HttpContext).Returns(httpContext.Object);

        var body = "{\"name\":\"张三\"}";
        var bytes = Encoding.GetEncoding("GBK").GetBytes(body);
        using var ms = new MemoryStream(bytes);
        mock.Setup(request => request.Body).Returns(ms);
        mock.Setup(request => request.ContentLength).Returns(ms.Length);

        var activity = new Activity("tets");
        activity.AddMasaSupplement(mock.Object);
        Assert.IsNotNull(activity);
        Assert.AreEqual("http", activity.GetTagItem(OpenTelemetryAttributeName.Http.SCHEME) as string);
        Assert.AreEqual("http1.1", activity.GetTagItem(OpenTelemetryAttributeName.Http.FLAVOR) as string);
        Assert.AreEqual(body, activity.GetTagItem(OpenTelemetryAttributeName.Http.REQUEST_CONTENT_BODY) as string);
    }

    [TestMethod]
    public void HttpResponseAddTagsTest()
    {
        Mock<HttpResponse> mock = new();
        mock.Setup(request => request.StatusCode).Returns(200);
        mock.Setup(request => request.ContentType).Returns(" application/json; charset=gbk");

        var httpContext = new Mock<HttpContext>();
        httpContext.Setup(context => context.User).Returns(new ClaimsPrincipal(new List<ClaimsIdentity> {
        new ClaimsIdentity( new Claim[]{ new Claim("sub","123456") },"userId"),
         new ClaimsIdentity( new Claim[]{ new Claim("https://masastack.com/security/authentication/MasaNickName", "admin") },"userName")
    }));
        mock.Setup(request => request.HttpContext).Returns(httpContext.Object);
        var body = "{\"name\":\"张三\"}";
        var bytes = Encoding.GetEncoding("GBK").GetBytes(body);
        using var ms = new MemoryStream(bytes);
        mock.Setup(request => request.Body).Returns(ms);
        mock.Setup(request => request.ContentLength).Returns(ms.Length);

        var activity = new Activity("tets");
        activity.AddMasaSupplement(mock.Object);
        Assert.IsNotNull(activity.GetTagItem(OpenTelemetryAttributeName.Http.RESPONSE_CONTENT_TYPE) as string);
    }
}

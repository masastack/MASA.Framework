// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Internal.Response;

internal class UploadObjectResponse
{
    public string ETag { get; set; }

    public string VersionId { get; set; }

    public string RequestId { get; set; }

    public HttpStatusCode HttpStatusCode { get; set; }

    public long ContentLength { get; set; }

    public string Response { get; set; }

    public IDictionary<string, string> ResponseMetadata { get; set; }

    public UploadObjectResponse(PutObjectResult result)
    {
        ETag = result.ETag;
        VersionId = result.VersionId;
        HttpStatusCode = result.HttpStatusCode;
        RequestId = result.RequestId;
        ContentLength = result.ContentLength;
        Response = GetCallbackResponse(result);
        ResponseMetadata = result.ResponseMetadata;
    }

    private string GetCallbackResponse(PutObjectResult putObjectResult)
    {
        using var stream = putObjectResult.ResponseStream;
        var buffer = new byte[4 * 1024];
        var bytesRead = stream.Read(buffer, 0, buffer.Length);
        string callbackResponse = Encoding.Default.GetString(buffer, 0, bytesRead);
        return callbackResponse;
    }
}

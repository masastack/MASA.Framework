// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

[TestClass]
public class ClientTest : TestBase
{
    private CustomStorageClientClient _client;

    [TestInitialize]
    public void Initialize()
    {
        _client = new CustomStorageClientClient(ALiYunStorageOptions, new MemoryCache(new MemoryDistributedCacheOptions()));
    }

    [TestMethod]
    public void TestGetToken()
    {
        Assert.ThrowsExactly<NotSupportedException>(() => _client.GetToken(), "GetToken is not supported, please use GetSecurityToken");
    }

    [TestMethod]
    public void TestGetSecurityTokenByCacheReturnSuccess()
    {
        Mock<ICredentialProvider> credentialProvider = new();
        TemporaryCredentialsResponse temporaryCredentials = new(
            "accessKeyId",
            "secretAccessKey",
            "sessionToken",
            DateTime.UtcNow.AddHours(-1));
        credentialProvider.Setup(provider => provider.GetSecurityToken()).Returns(temporaryCredentials);

        var aliyunStorageOptions = new AliyunStorageOptions()
        {
            Sts=new AliyunStsOptions("regionId"),
            RoleArn = "roleArn",
            RoleSessionName = "roleSessionName"
        };
        var client = new DefaultStorageClient(
            aliyunStorageOptions,
            new MemoryCache(new MemoryDistributedCacheOptions()),
            credentialProvider.Object);
        var responseBase = client.GetSecurityToken();
        Assert.IsTrue(responseBase == temporaryCredentials);
    }

    [TestMethod]
    public void TestEmptyRoleArnGetSecurityTokenReturnThrowArgumentException()
    {
        Mock<ICredentialProvider> credentialProvider = new();
        var aliyunStorageOptions = new AliyunStorageOptions("AccessKeyId", "AccessKeySecret", HANG_ZHOUE_PUBLIC_ENDPOINT);
        var client = new DefaultStorageClient(aliyunStorageOptions,new MemoryCache(new MemoryDistributedCacheOptions()),credentialProvider.Object);
        Assert.ThrowsExactly<ArgumentException>(() => client.GetSecurityToken());
    }

    [TestMethod]
    public async Task TestGetObjectAsyncReturnGetObjecVerifytOnce()
    {
        await _client.GetObjectAsync("bucketName", "objectName", stream =>
        {
            Assert.IsTrue(stream == null);
        });
        _client.Oss!.Verify(oss => oss.GetObject(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public async Task TestGetObjectAsyncByOffsetReturnGetObjecVerifytOnce()
    {
        await _client.GetObjectAsync("bucketName", "objectName", 1, 2, stream =>
        {
            Assert.IsTrue(stream == null);
        });
        _client.Oss!.Verify(oss => oss.GetObject(It.IsAny<GetObjectRequest>()), Times.Once);
    }

    [TestMethod]
    public async Task TestGetObjectAsyncByLengthLessThan0AndNotEqualMinus1ReturnThrowArgumentOutOfRangeException()
    {
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async ()
            => await _client.GetObjectAsync("bucketName", "objectName", 1, -2, null!));
    }

    [TestMethod]
    public async Task TestPutObjectAsyncReturnPutObjectVerifytOnce()
    {
        string str = "JIm";
        await _client.PutObjectAsync("bucketName", "objectName", new MemoryStream(Encoding.Default.GetBytes(str)));
        _client.Oss!.Verify(oss => oss.PutObject(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<ObjectMetadata>()),
            Times.Once);
    }

    [TestMethod]
    public async Task TestPutObjectAsyncReturnResumableUploadObjectVerifytOnce()
    {
        ALiYunStorageOptions.BigObjectContentLength = 2;
        string str = "JIm";
        await _client.PutObjectAsync("bucketName", "objectName", new MemoryStream(Encoding.Default.GetBytes(str)));
        _client.Oss!.Verify(oss => oss.ResumableUploadObject(It.IsAny<UploadObjectRequest>()), Times.Once);
    }

    [TestMethod]
    public async Task TestObjectExistsAsyncReturnNotFound()
    {
        Assert.IsFalse(await _client.ObjectExistsAsync("bucketName", "1.jpg"));
    }

    [TestMethod]
    public async Task TestObjectExistsAsyncReturnExist()
    {
        Assert.IsTrue(await _client.ObjectExistsAsync("bucketName", "2.jpg"));
    }

    [TestMethod]
    public async Task TestDeleteObjectAsyncReturnVerifytNever()
    {
        await _client.DeleteObjectAsync("bucketName", "1.jpg");
        _client.Oss!.Verify(oss => oss.DeleteObject(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task TestDeleteObjectAsyncReturnVerifytOnce()
    {
        await _client.DeleteObjectAsync("bucketName", "2.jpg");
        _client.Oss!.Verify(oss => oss.DoesObjectExist(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _client.Oss!.Verify(oss => oss.DeleteObject(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public async Task TestDeleteMultiObjectAsyncReturnVerifytOnce()
    {
        await _client.DeleteObjectAsync("bucketName", new[] { "2.jpg", "1.jpg" });
        _client.Oss!.Verify(oss => oss.DeleteObjects(It.IsAny<DeleteObjectsRequest>()), Times.Once);
    }
}

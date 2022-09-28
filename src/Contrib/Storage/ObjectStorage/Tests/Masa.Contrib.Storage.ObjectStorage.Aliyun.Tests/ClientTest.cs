// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

[TestClass]
public class ClientTest : TestBase
{
    private CustomClient _client;

    [TestInitialize]
    public void Initialize()
    {
        Mock<ICredentialProvider> credentialProvider = new Mock<ICredentialProvider>();
        Mock<IAliyunStorageOptionProvider> optionProvider = MockOptionProvider(true);
        _client = new CustomClient(credentialProvider.Object, optionProvider.Object, NullLogger<DefaultStorageClient>.Instance);
    }

    [TestMethod]
    public void TestGetTokenAndNullLoggerReturnFalse()
    {
        Mock<ICredentialProvider> credentialProvider = new();
        var client = new DefaultStorageClient(credentialProvider.Object, MockOptionProvider(false).Object, null);
        Assert.ThrowsException<NotSupportedException>(() => client.GetToken(), "GetToken is not supported, please use GetSecurityToken");
    }

    [TestMethod]
    public void TestGetTokenAndNotNullLoggerReturnFalse()
    {
        Mock<ICredentialProvider> credentialProvider = new();
        var client = new DefaultStorageClient(credentialProvider.Object, MockOptionProvider().Object, NullLogger<DefaultStorageClient>.Instance);
        Assert.ThrowsException<NotSupportedException>(() => client.GetToken(), "GetToken is not supported, please use GetSecurityToken");
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
        var client = new DefaultStorageClient(credentialProvider.Object, MockOptionProvider(false).Object, NullLogger<DefaultStorageClient>.Instance);
        var responseBase = client.GetSecurityToken();
        Assert.IsTrue(responseBase == temporaryCredentials);
    }

    [TestMethod]
    public void TestEmptyRoleArnGetSecurityTokenReturnThrowArgumentException()
    {
        Mock<ICredentialProvider> credentialProvider = new();
        _aLiYunStorageOptions = new AliyunStorageOptions("AccessKeyId", "AccessKeySecret", HANG_ZHOUE_PUBLIC_ENDPOINT);
        var client = new DefaultStorageClient(credentialProvider.Object, MockOptionProvider(true).Object, NullLogger<DefaultStorageClient>.Instance);
        Assert.ThrowsException<ArgumentException>(() => client.GetSecurityToken());
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
        await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async ()
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
        _aLiYunStorageOptions.BigObjectContentLength = 2;
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

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

#pragma warning disable S2326
public interface IObjectStorageClientContainer<TContainer> : IObjectStorageClientContainer where TContainer : class
{

}
#pragma warning restore S2326

public interface IObjectStorageClientContainer
{
    /// <summary>
    /// Obtain temporary authorization credentials through STS service
    /// </summary>
    /// <returns></returns>
    TemporaryCredentialsResponse GetSecurityToken();

    /// <summary>
    /// Obtain temporary request token through authorization service
    /// </summary>
    /// <returns></returns>
    string GetToken();

    Task GetObjectAsync(
        string objectName,
        Action<Stream> callback,
        CancellationToken cancellationToken = default);

    Task GetObjectAsync(
        string objectName,
        long offset,
        long length,
        Action<Stream> callback,
        CancellationToken cancellationToken = default);

    Task PutObjectAsync(
        string objectName,
        Stream data,
        CancellationToken cancellationToken = default);

    Task<bool> ObjectExistsAsync(
        string objectName,
        CancellationToken cancellationToken = default);

    Task DeleteObjectAsync(
        string objectName,
        CancellationToken cancellationToken = default);

    Task DeleteObjectAsync(
        IEnumerable<string> objectNames,
        CancellationToken cancellationToken = default);
}

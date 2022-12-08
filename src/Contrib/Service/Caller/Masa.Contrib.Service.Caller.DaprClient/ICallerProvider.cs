// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient;

public interface ICallerProvider
{
    /// <summary>
    /// According to the dapr appid obtained by the service id
    /// when the dapr appid of the specified service does not exist in the configuration, return the service id as the dapr appid
    /// </summary>
    /// <param name="appId">service appid</param>
    /// <returns></returns>
    string CompletionAppId(string appId);
}

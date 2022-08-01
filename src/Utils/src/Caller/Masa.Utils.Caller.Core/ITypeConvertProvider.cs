// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Core;

public interface ITypeConvertProvider
{
    /// <summary>
    /// Convert custom object to dictionary
    /// </summary>
    /// <param name="request"></param>
    /// <typeparam name="TRequest">Support classes, anonymous objects</typeparam>
    /// <returns></returns>
    [Obsolete("Use ConvertToKeyValuePairs instead")]
    Dictionary<string, string> ConvertToDictionary<TRequest>(TRequest request) where TRequest : class;

    /// <summary>
    /// Convert custom object to dictionary
    /// </summary>
    /// <param name="request"></param>
    /// <typeparam name="TRequest">Support classes, anonymous objects</typeparam>
    /// <returns></returns>
    IEnumerable<KeyValuePair<string, string>> ConvertToKeyValuePairs<TRequest>(TRequest request) where TRequest : class;
}

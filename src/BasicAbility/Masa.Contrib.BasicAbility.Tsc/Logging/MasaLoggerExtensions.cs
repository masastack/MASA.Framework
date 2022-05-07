// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.Logging;

public static class MasaLoggerExtensions
{
    private static bool IsNull(ILogger logger) => logger == null;

    public static IDisposable? LogMasaAttr(this ILogger logger, string key, params object[] value)
    {
        if (IsNull(logger) || string.IsNullOrEmpty(key))
            return default;

        return logger.BeginScope(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>(key, value) });
    }
}

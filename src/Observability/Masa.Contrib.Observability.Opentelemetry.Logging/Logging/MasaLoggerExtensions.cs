namespace Microsoft.Extensions.Logging;

public static class MasaLoggerExtensions
{
    private static bool IsNull(ILogger logger) => logger == null;

    public static void LogMasaAttr(this ILogger logger, string key, object value)
    {
        if (IsNull(logger) || string.IsNullOrEmpty(key))
            return;

        logger.BeginScope(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>(key, value) });
    }
}

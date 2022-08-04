namespace Masa.Utils.Caching.Redis.Internal.Enum;

internal enum CompressMode
{
    /// <summary>
    /// no compression
    /// </summary>
    None = 1,

    /// <summary>
    /// Compress but not deserialize
    /// </summary>
    Compress,

    /// <summary>
    /// serialize and compress
    /// </summary>
    SerializeAndCompress,
}

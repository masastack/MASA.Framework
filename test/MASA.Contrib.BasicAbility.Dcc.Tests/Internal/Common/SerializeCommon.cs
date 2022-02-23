namespace MASA.Contrib.BasicAbility.Dcc.Tests.Internal.Common;

internal static class SerializeCommon
{
    public static string Serialize(this object obj, JsonSerializerOptions? jsonSerializerOptions)
        => JsonSerializer.Serialize(obj, jsonSerializerOptions);
}

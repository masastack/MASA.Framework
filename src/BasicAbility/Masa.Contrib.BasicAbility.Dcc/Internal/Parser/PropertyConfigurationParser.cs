namespace Masa.Contrib.BasicAbility.Dcc.Internal.Parser;

internal class PropertyConfigurationParser
{
    public static IDictionary<string, string>? Parse(string raw, JsonSerializerOptions serializerOption)
        => JsonSerializer.Deserialize<List<Property>>(raw, serializerOption)?.ToDictionary(k => k.Key, v => v.Value);
}

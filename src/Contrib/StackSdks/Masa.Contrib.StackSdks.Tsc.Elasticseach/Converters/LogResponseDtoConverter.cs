// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Log.Elasticseach.Converters;

internal class LogResponseDtoConverter : JsonConverter<LogResponseDto>
{
    public override LogResponseDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (JsonDocument.TryParseValue(ref reader, out var doc))
        {
            var jsonObject = doc.RootElement;
            var rootText = jsonObject.GetRawText();
            var result= JsonSerializer.Deserialize<ElasticseachLogResponseDto>(rootText);
            if (result == null)
                return default;            
            if (result.Timestamp == DateTime.MinValue || result.Timestamp == DateTime.MaxValue)
                return default;
            if (result.Body == null)
                return default;

            result.Attributes = jsonObject.ToKeyValuePairs()?.ToDictionary(kv => kv.Key, kv => kv.Value)?.ConvertDic<object>("Attributes.")!;
            result.Resource = jsonObject.ToKeyValuePairs()?.ToDictionary(kv => kv.Key, kv => kv.Value)?.ConvertDic<object>("Resource.")!;

            return result;
        }

        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, LogResponseDto value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}

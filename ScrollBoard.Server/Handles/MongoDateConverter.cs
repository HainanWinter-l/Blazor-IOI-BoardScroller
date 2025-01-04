using System.Text.Json.Serialization;
using System.Text.Json;

namespace ScrollBoard.Server.Handles;

public class MongoDateConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        return root.TryGetProperty("$date", out var dateValue) ?
            // MongoDB 的日期是 ISO 8601 格式
            DateTime.Parse(dateValue.GetString() ?? string.Empty) : default;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("$date", value.ToString("o"));  // 使用 ISO 8601 格式输出
        writer.WriteEndObject();
    }
}



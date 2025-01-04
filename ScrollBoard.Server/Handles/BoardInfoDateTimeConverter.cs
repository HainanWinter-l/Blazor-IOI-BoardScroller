using System.Text.Json.Serialization;
using System.Text.Json;

namespace ScrollBoard.Server.Handles
{
    public class BoardInfoDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("Invalid DateTime format.");
            }

            var dateStr = reader.GetString();
            if (DateTime.TryParse(dateStr, out var date))
            {
                return date;
            }

            throw new JsonException("Invalid DateTime format.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;
using FluentResults;

namespace Analytics.Infrastructure.Caching.Json;

public class ResultJsonConverter<T> : JsonConverter<Result<T>>
{
    public override Result<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var isSuccess = root.TryGetProperty("IsSuccess", out var s) && s.GetBoolean();
        
        if (isSuccess)
        {
            var valueElement = root.GetProperty("Value");
            var value = JsonSerializer.Deserialize<T>(valueElement.GetRawText(), options);
            return Result.Ok(value!);
        }

        var errors = new List<IError>();
        if (root.TryGetProperty("Errors", out var errorsElement))
        {
            foreach (var err in errorsElement.EnumerateArray())
            {
                var msg = err.GetProperty("Message").GetString() ?? "Unknown error";
                errors.Add(new Error(msg));
            }
        }
        return Result.Fail<T>(errors);
    }

    public override void Write(Utf8JsonWriter writer, Result<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteBoolean("IsSuccess", value.IsSuccess);
        writer.WriteBoolean("IsFailed", value.IsFailed);

        if (value.IsSuccess)
        {
            writer.WritePropertyName("Value");
            JsonSerializer.Serialize(writer, value.Value, options);
        }

        writer.WritePropertyName("Errors");
        writer.WriteStartArray();
        foreach (var err in value.Errors)
        {
            writer.WriteStartObject();
            writer.WriteString("Message", err.Message);
            writer.WriteEndObject();
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }
}
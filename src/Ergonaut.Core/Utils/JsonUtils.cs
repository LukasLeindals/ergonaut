using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Ergonaut.Core.Utils;

public static class JsonUtils
{
    public static JsonElement? ConvertToJsonElement(string? value, ILogger? logger = null)
    {
        if (value is null)
            return null;

        if (value.Length > 0 && IsJsonLike(value[0]))
        {
            bool hadError = false;
            try
            {
                var utf8 = Encoding.UTF8.GetBytes(value);
                var reader = new Utf8JsonReader(utf8);
                if (JsonDocument.TryParseValue(ref reader, out var doc))
                {
                    using (doc)
                        return doc.RootElement.Clone();
                }
            }
            catch
            {
                // ignore parse errors and fallback to JSON string
                hadError = true;
            }
            if (logger != null && hadError)
            {
                logger.LogWarning("Failed to parse value as JSON: {Value}", value);
            }
        }


        // fallback: JSON string
        using var quoted = JsonDocument.Parse(JsonSerializer.Serialize(value));
        return quoted.RootElement.Clone();
    }

    private static bool IsJsonLike(char c) =>
        c is '{' or '[' or '"' or 't' or 'f' or 'n' or '-' or (>= '0' and <= '9');

}

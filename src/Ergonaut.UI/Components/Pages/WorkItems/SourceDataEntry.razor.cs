using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Ergonaut.UI.Components.Pages.WorkItems;

public class SourceDataEntry
{
    [Required]
    public string Key { get; set; } = string.Empty;

    public string? Value { get; set; } = default;

    public Dictionary<string, JsonElement?> AddEntry(Dictionary<string, JsonElement?>? sourceData)
    {

        sourceData ??= new();

        // Validate Key and Value
        if (string.IsNullOrWhiteSpace(Key))
        {
            throw new ValidationException("Key is required.");
        }
        if (sourceData.ContainsKey(Key))
        {
            throw new ValidationException($"An entry with the key '{Key}' already exists.");
        }

        if (Value == null)
        {
            sourceData[Key] = null;
            return sourceData;
        }

        try
        {
            using var jsonDoc = JsonDocument.Parse(Value);
            sourceData[Key] = jsonDoc.RootElement.Clone();
        }
        catch (JsonException)
        {
            throw new ValidationException("Value must be valid JSON (wrap strings in quotes).");
        }
        return sourceData;
    }
}

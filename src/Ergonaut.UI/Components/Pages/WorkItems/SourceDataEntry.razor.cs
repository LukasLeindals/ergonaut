using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Ergonaut.Core.Utils;

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

        sourceData[Key] = JsonUtils.ConvertToJsonElement(Value);

        return sourceData;
    }
}

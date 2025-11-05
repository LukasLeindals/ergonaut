using System.Text.Json;
using Ergonaut.Core.LogIngestion;


namespace Ergonaut.App.LogIngestion;


public sealed class KafkaLogEventEnvelope
{
    public ILogEvent Data { get; }

    public static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };


    public KafkaLogEventEnvelope(ILogEvent data)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }


    public byte[] Wrap()
    {
        return JsonSerializer.SerializeToUtf8Bytes(Data, JsonOptions);
    }

    public static bool TryUnwrap(byte[] data, out KafkaLogEventEnvelope? envelope)
    {
        try
        {
            envelope = Unwrap(data);
            return true;
        }
        catch
        {
            envelope = null;
            return false;
        }
    }


    public static KafkaLogEventEnvelope Unwrap(byte[] data)
    {
        var logEvent = JsonSerializer.Deserialize<LogEvent>(data, JsonOptions);
        return new KafkaLogEventEnvelope(logEvent!);
    }

}

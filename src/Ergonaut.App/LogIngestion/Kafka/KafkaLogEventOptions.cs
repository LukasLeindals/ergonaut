using System.ComponentModel.DataAnnotations;
using System;

namespace Ergonaut.App.LogIngestion.Kafka;

public sealed class KafkaLogEventOptions
{
    [Required]
    public string Topic { get; set; }

    [Required]
    public string BootstrapServers { get; set; }

    [Required]
    public string GroupId { get; set; }

    /// <summary>
    /// Maximum number of parallel message handlers.
    /// </summary>
    [Range(1, 256)]
    public int MaxParallelism { get; set; } = Environment.ProcessorCount;


    public KafkaLogEventOptions(string bootstrapServers, string topic, string groupId)
    {
        BootstrapServers = bootstrapServers;
        Topic = topic;
        GroupId = groupId;
    }

    public KafkaLogEventOptions()
    {
        BootstrapServers = string.Empty;
        Topic = string.Empty;
        GroupId = string.Empty;
    }
}

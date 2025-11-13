using System.Text.Json;
using Ergonaut.Core.Models.WorkItem;
using Ergonaut.Core.Models;

namespace Ergonaut.App.Models;

public sealed record WorkItemRecord : IWorkItem
{
    public required Guid Id { get; init; }

    public required Guid ProjectId { get; init; }

    public required string Title { get; init; }

    public string? Description { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }

    public WorkItemStatus Status { get; init; }

    public WorkItemPriority? Priority { get; init; }

    public DateTime? DueDate { get; init; }

    public SourceLabel? SourceLabel { get; init; }

    public Dictionary<string, JsonElement?>? SourceData { get; init; } = null;

    public IWorkItem Update(string title, WorkItemStatus status, string? description = null, WorkItemPriority? priority = null, DateTime? dueDate = null, SourceLabel? sourceLabel = null, Dictionary<string, JsonElement?>? sourceData = null)
    {
        return this with
        {
            Title = title,
            Status = status,
            Description = description,
            Priority = priority,
            DueDate = dueDate,
            SourceLabel = sourceLabel,
            SourceData = sourceData
        };
    }
    public static WorkItemRecord FromWorkItem(IWorkItem workItem) => new()
    {
        Id = workItem.Id,
        ProjectId = workItem.ProjectId,
        Title = workItem.Title,
        Description = workItem.Description,
        CreatedAt = workItem.CreatedAt,
        UpdatedAt = workItem.UpdatedAt,
        Status = workItem.Status,
        Priority = workItem.Priority,
        DueDate = workItem.DueDate,
        SourceLabel = workItem.SourceLabel,
        SourceData = workItem.SourceData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
    };
}

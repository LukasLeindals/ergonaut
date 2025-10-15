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

    public SourceLabel Source { get; init; }

    public string? SourceData { get; init; } = null;

    public static WorkItemRecord FromWorkItem(IWorkItem workItem) => new()
    {
        Id = workItem.Id,
        ProjectId = workItem.ProjectId,
        Title = workItem.Title,
        Description = workItem.Description,
        CreatedAt = workItem.CreatedAt,
        UpdatedAt = workItem.UpdatedAt,
        Source = workItem.Source
    };
}

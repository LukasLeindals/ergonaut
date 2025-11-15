using System.Text.Json;
using Ergonaut.Core.Models.WorkItem;

namespace Ergonaut.Core.Models.WorkItem;

/// <summary>
/// Represents a work item in the Ergonaut planning system.
/// </summary>
public partial class WorkItem : IWorkItem
{

    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid ProjectId { get; private set; }

    public string Title { get; private set; }

    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the priority level of the work item.
    /// </summary>
    public WorkItemPriority? Priority { get; private set; } = null;

    /// <summary>
    /// Gets or sets the current status of the work item.
    /// </summary>
    public WorkItemStatus Status { get; private set; } = WorkItemStatus.New;

    /// <summary>
    /// Gets or sets the date and time when the work item was created.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the work item was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public SourceLabel? SourceLabel { get; private set; } = null;
    public Dictionary<string, JsonElement?>? SourceData { get; private set; } = null;

    /// <summary>
    /// Gets or sets the due date for the work item.
    /// </summary>
    public DateTime? DueDate { get; private set; } = null;

    /// <summary>
    /// Base constructor for the work item class.
    /// </summary>
    /// <param name="projectId">The identifier of the project this work item belongs to.</param>
    /// <param name="title">The title of the work item.</param>
    public WorkItem(
        Guid projectId, string title, SourceLabel? sourceLabel,
        string? description = null, WorkItemStatus status = WorkItemStatus.New, WorkItemPriority? priority = null, DateTime? dueDate = null, Dictionary<string, JsonElement?>? sourceData = null
        )
    {
        ProjectId = projectId;
        Title = NormalizeTitle(title);
        SourceLabel = sourceLabel;
        Description = NormalizeDescription(description);
        SourceData = sourceData;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
    }

    public IWorkItem Update(string title, WorkItemStatus status, string? description = null, WorkItemPriority? priority = null, DateTime? dueDate = null, SourceLabel? sourceLabel = null, Dictionary<string, JsonElement?>? sourceData = null)
    {

        Title = NormalizeTitle(title);
        Description = NormalizeDescription(description);
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        SourceLabel = sourceLabel;
        SourceData = sourceData;

        UpdatedAt = DateTime.UtcNow;

        return this;
    }

    private static string NormalizeTitle(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        return title.Trim();
    }

    private static string NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return string.Empty;
        }

        return description.Trim();
    }
}


using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Ergonaut.Core.Models;
using Ergonaut.Core.Models.WorkItem;

namespace Ergonaut.App.Models;

public sealed class UpdateWorkItemRequest
{

    [Required(ErrorMessage = "Please give your work item a title.")]
    [StringLength(200, ErrorMessage = "Keep the name under 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Keep the description under 1000 characters.")]
    public string? Description { get; set; }

    public SourceLabel? SourceLabel { get; set; } = null;

    public WorkItemStatus Status { get; set; } = WorkItemStatus.New;

    public WorkItemPriority? Priority { get; set; } = null;

    public DateTime? DueDate { get; set; } = null;

    public Dictionary<string, JsonElement?>? SourceData { get; set; } = null;

    public UpdateWorkItemRequest() { }
    public UpdateWorkItemRequest(SourceLabel? sourceLabel = null)
    {
        SourceLabel = sourceLabel;
    }

    public static UpdateWorkItemRequest FromWorkItem(IWorkItem workItem)
    {
        return new UpdateWorkItemRequest(workItem.SourceLabel)
        {
            Title = workItem.Title,
            Description = workItem.Description,
            Status = workItem.Status,
            Priority = workItem.Priority,
            DueDate = workItem.DueDate,
            SourceLabel = workItem.SourceLabel,
            SourceData = workItem.SourceData
        };
    }
}

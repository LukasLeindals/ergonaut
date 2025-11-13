using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Ergonaut.Core.Models;
using Ergonaut.Core.Models.WorkItem;

namespace Ergonaut.App.Models;

public sealed class CreateWorkItemRequest
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

    public CreateWorkItemRequest() { }
    public CreateWorkItemRequest(SourceLabel? sourceLabel = null)
    {
        SourceLabel = sourceLabel;
    }
}

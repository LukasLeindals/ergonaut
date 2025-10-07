using IWorkItem = Ergonaut.Core.Models.WorkItem.IWorkItem;
using System.ComponentModel.DataAnnotations;

namespace Ergonaut.App.Features.WorkItems;

public sealed record WorkItemSummary(Guid ProjectId, Guid Id, string Title, string? Description)
{
    public static WorkItemSummary FromWorkItem(IWorkItem workItem) =>
        new(workItem.ProjectId, workItem.Id, workItem.Title, workItem.Description ?? string.Empty);
}

public sealed class CreateWorkItemRequest
{

    [Required(ErrorMessage = "Please give your work item a title.")]
    [StringLength(200, ErrorMessage = "Keep the name under 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Keep the description under 1000 characters.")]
    public string? Description { get; set; }
}



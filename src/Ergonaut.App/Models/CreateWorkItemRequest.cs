using System.ComponentModel.DataAnnotations;
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

    [Required(ErrorMessage = "Please specify the source of the work item.")]
    public SourceLabel Source { get; set; }
}

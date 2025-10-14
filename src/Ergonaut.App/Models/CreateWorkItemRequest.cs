using System.ComponentModel.DataAnnotations;

namespace Ergonaut.App.Models;
public sealed class CreateWorkItemRequest
{

    [Required(ErrorMessage = "Please give your work item a title.")]
    [StringLength(200, ErrorMessage = "Keep the name under 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Keep the description under 1000 characters.")]
    public string? Description { get; set; }
}

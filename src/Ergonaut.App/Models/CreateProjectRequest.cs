using System.ComponentModel.DataAnnotations;
using Ergonaut.Core.Models;

namespace Ergonaut.App.Models;

public sealed class CreateProjectRequest
{
    [Required(ErrorMessage = "Please give your project a name.")]
    [StringLength(200, ErrorMessage = "Keep the name under 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Keep the description under 1000 characters.")]
    public string? Description { get; set; }

    public SourceLabel? SourceLabel { get; set; } = null;
}

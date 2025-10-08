using System.ComponentModel.DataAnnotations;

namespace Ergonaut.App.Models;
public sealed class CreateProjectRequest
{
    [Required(ErrorMessage = "Please give your project a name.")]
    [StringLength(200, ErrorMessage = "Keep the name under 200 characters.")]
    public string Title { get; set; } = string.Empty;
}

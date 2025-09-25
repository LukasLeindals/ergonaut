using DomainProject = Ergonaut.Core.Models.Project;
using System.ComponentModel.DataAnnotations;

namespace Ergonaut.UI.Features.Projects;

public sealed record ProjectSummary(Guid Id, string Title, DateTime CreatedAt, DateTime UpdatedAt)
{
    public static ProjectSummary FromDomain(DomainProject project) =>
        new(project.Id, project.Title, project.CreatedAt, project.UpdatedAt);
}

public sealed class CreateProjectRequest
{
    [Required(ErrorMessage = "Please give your project a name.")]
    [StringLength(200, ErrorMessage = "Keep the name under 200 characters.")]
    public string Title { get; set; } = string.Empty;
}

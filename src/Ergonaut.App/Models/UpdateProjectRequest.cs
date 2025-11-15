using Ergonaut.Core.Models.Project;
using Ergonaut.Core.Models;

namespace Ergonaut.App.Models;

public sealed class UpdateProjectRequest
{

    public required string Title { get; set; }

    public string? Description { get; set; }

    public SourceLabel? SourceLabel { get; set; }

    public static UpdateProjectRequest FromProject(IProject project) => new()
    {
        Title = project.Title,
        Description = project.Description,
        SourceLabel = project.SourceLabel
    };
}

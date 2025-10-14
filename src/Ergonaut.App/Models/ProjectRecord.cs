using Ergonaut.Core.Models.Project;
using Ergonaut.Core.Models;

namespace Ergonaut.App.Models;

public sealed record ProjectRecord : IProject
{
    public required Guid Id { get; init; }

    public required string Title { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }

    public SourceLabel Source { get; init; }

    public static ProjectRecord FromProject(IProject project) => new()
    {
        Id = project.Id,
        Title = project.Title,
        CreatedAt = project.CreatedAt,
        UpdatedAt = project.UpdatedAt,
        Source = project.Source
    };
}

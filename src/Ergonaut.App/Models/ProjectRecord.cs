using Ergonaut.Core.Models.Project;
using Ergonaut.Core.Models;

namespace Ergonaut.App.Models;

public sealed record ProjectRecord : IProject
{
    public required Guid Id { get; init; }

    public required string Title { get; init; }

    public string? Description { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }

    public SourceLabel? SourceLabel { get; init; }

    public static ProjectRecord FromProject(IProject project) => new()
    {
        Id = project.Id,
        Title = project.Title,
        Description = project.Description,
        CreatedAt = project.CreatedAt,
        UpdatedAt = project.UpdatedAt,
        SourceLabel = project.SourceLabel
    };

    public IProject Update(string title, string? description, SourceLabel? sourceLabel)
    {
        return this with
        {
            Title = title,
            Description = description,
            SourceLabel = sourceLabel
        };
    }
}

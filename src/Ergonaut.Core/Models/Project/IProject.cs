using Ergonaut.Core.Models;
namespace Ergonaut.Core.Models.Project;

public interface IProject
{
    Guid Id { get; }
    string Title { get; }
    string? Description { get; }
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; }
    SourceLabel? SourceLabel { get; }

    IProject Update(string title, string? description, SourceLabel? sourceLabel);
}

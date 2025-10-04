namespace Ergonaut.Core.Models.Project;

public interface IProject
{
    Guid Id { get; }
    string Title { get; }
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; }
    ProjectSource Source { get; }
}

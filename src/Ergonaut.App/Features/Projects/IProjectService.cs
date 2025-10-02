namespace Ergonaut.App.Features.Projects;

public interface IProjectService
{
    // Lists all projects, ordered by creation date descending.
    Task<IReadOnlyList<ProjectSummary>> ListAsync(CancellationToken ct = default);

    // Creates a new project from the given request.
    Task<ProjectSummary> CreateAsync(CreateProjectRequest request, CancellationToken ct = default);

    // Deletes a project by its ID.
    Task<DeletionResult> DeleteAsync(Guid id, CancellationToken ct = default);

    // Retrieves a project by its ID, or null if not found.
    Task<ProjectSummary?> GetAsync(Guid id, CancellationToken ct = default); // DTO return type
}

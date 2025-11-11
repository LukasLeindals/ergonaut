using Ergonaut.App.Models;

namespace Ergonaut.App.Services;

public interface IProjectService
{
    // Lists all projects, ordered by creation date descending.
    Task<IReadOnlyList<ProjectRecord>> ListAsync(CancellationToken ct = default);

    // Creates a new project from the given request.
    Task<ProjectRecord> CreateAsync(CreateProjectRequest request, CancellationToken ct = default);

    // Deletes a project by its ID.
    Task<DeletionResponse> DeleteAsync(Guid id, CancellationToken ct = default);

    // Retrieves a project by its ID, or null if not found.
    Task<ProjectRecord?> GetAsync(Guid id, CancellationToken ct = default); // DTO return type

    Task<ProjectRecord?> GetProjectByName(string projectName, CancellationToken ct = default);
}

using Ergonaut.Infrastructure.Repositories;

namespace Ergonaut.App.Features.Projects;

public interface IProjectService
{
    // Lists all projects, ordered by creation date descending.
    Task<IReadOnlyList<ProjectSummary>> ListAsync(CancellationToken ct = default);

    // Creates a new project from the given request.
    Task<ProjectSummary> CreateAsync(CreateProjectRequest request, CancellationToken ct = default);

    // Retrieves a project by its ID, or null if not found.
    Task<ProjectSummary?> GetAsync(Guid id, CancellationToken ct = default); // DTO return type
}

public sealed class ProjectService(IProjectRepository repository) : IProjectService
{
    public async Task<IReadOnlyList<ProjectSummary>> ListAsync(CancellationToken ct = default) =>
        (await repository.ListAsync(ct))
            .Select(ProjectSummary.FromDomain)
            .OrderByDescending(p => p.CreatedAt)
            .ToArray();

    public async Task<ProjectSummary> CreateAsync(CreateProjectRequest request, CancellationToken ct = default)
    {
        var project = new Ergonaut.Core.Models.Project(request.Title);
        var saved = await repository.AddAsync(project, ct);
        return ProjectSummary.FromDomain(saved);
    }

    public async Task<ProjectSummary?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var project = await repository.GetAsync(id, ct); // repository stays domain-focused
        return project is null ? null : ProjectSummary.FromDomain(project);
    }
}

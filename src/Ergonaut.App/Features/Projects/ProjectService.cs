using Ergonaut.Core.Repositories;
using Ergonaut.App.Models;

namespace Ergonaut.App.Features.Projects;

public sealed class ProjectService(IProjectRepository repository, IProjectFactory projectFactory) : IProjectService
{
    public async Task<IReadOnlyList<ProjectSummary>> ListAsync(CancellationToken ct = default) =>
        (await repository.ListAsync(ct))
            .Select(ProjectSummary.FromProject)
            .OrderByDescending(p => p.CreatedAt)
            .ToArray();

    public async Task<ProjectSummary> CreateAsync(CreateProjectRequest request, CancellationToken ct = default)
    {
        var project = projectFactory.Create(request.Title);
        var saved = await repository.AddAsync(project, ct);
        return ProjectSummary.FromProject(saved);
    }

    public async Task<DeletionResult> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await repository.GetAsync(id, ct);
        if (existing is null)
            return new DeletionResult(false, "Project not found.");

        await repository.DeleteAsync(id, ct);
        return new DeletionResult(true, "Project deleted successfully.");
    }

    public async Task<ProjectSummary?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var project = await repository.GetAsync(id, ct); // repository stays domain-focused
        return project is null ? null : ProjectSummary.FromProject(project);
    }
}

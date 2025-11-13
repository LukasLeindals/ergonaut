using Ergonaut.Core.Repositories;
using Ergonaut.Core.Models.Project;
using Ergonaut.App.Models;

namespace Ergonaut.App.Services;

public sealed class ProjectService(IProjectRepository repository) : IProjectService
{
    public async Task<IReadOnlyList<ProjectRecord>> ListAsync(CancellationToken ct = default) =>
        (await repository.ListAsync(ct))
            .Select(ProjectRecord.FromProject)
            .OrderByDescending(p => p.CreatedAt)
            .ToArray();

    public async Task<ProjectRecord> CreateAsync(CreateProjectRequest request, CancellationToken ct = default)
    {
        IProject project = await repository.AddAsync(new Project(title: request.Title, description: request.Description, sourceLabel: request.SourceLabel), ct);
        return ProjectRecord.FromProject(project);
    }

    public async Task<DeletionResponse> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        IProject? existing = await repository.GetAsync(id, ct);
        if (existing is null)
            return new DeletionResponse(false, "Project not found.");

        await repository.DeleteAsync(id, ct);
        return new DeletionResponse(true, "Project deleted successfully.");
    }

    public async Task<ProjectRecord?> GetAsync(Guid id, CancellationToken ct = default)
    {
        IProject? project = await repository.GetAsync(id, ct); // repository stays domain-focused
        return project is null ? null : ProjectRecord.FromProject(project);
    }

    public async Task<ProjectRecord?> GetProjectByName(string projectName, CancellationToken ct = default)
    {
        IProject? project = await repository.GetByNameAsync(projectName, ct);
        return project is null ? null : ProjectRecord.FromProject(project);
    }
}

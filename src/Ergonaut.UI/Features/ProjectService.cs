using Ergonaut.Infrastructure.Repositories;

namespace Ergonaut.UI.Features.Projects;

public interface IProjectService
{
    Task<IReadOnlyList<ProjectSummary>> ListAsync(CancellationToken ct = default);
    Task<ProjectSummary> CreateAsync(CreateProjectRequest request, CancellationToken ct = default);
}

internal sealed class ProjectService(IProjectRepository repository) : IProjectService
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
}

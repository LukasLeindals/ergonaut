using Ergonaut.App.Features.Projects;

namespace Ergonaut.UI.Features.Projects;

internal sealed class ProjectInfoQuery(IProjectService inner)
{
    public async Task<IReadOnlyList<ProjectInfo>> ListAsync(CancellationToken ct = default)
    {
        var projects = await inner.ListAsync(ct);
        return projects.Select(ToInfo).ToList();
    }

    public async Task<ProjectInfo?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var project = await inner.GetAsync(id, ct);
        return project is null ? null : ToInfo(project);
    }

    public async Task<ProjectInfo> CreateAsync(CreateProjectRequest request, CancellationToken ct = default)
    {
        var project = await inner.CreateAsync(request, ct);
        return ToInfo(project);
    }

    private static ProjectInfo ToInfo(ProjectSummary project) =>
        new(project.Id, project.Title, project.CreatedAt);
}

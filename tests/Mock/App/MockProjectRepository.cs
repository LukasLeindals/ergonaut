using Ergonaut.Core.Models.Project;
using Ergonaut.Core.Repositories;

namespace Ergonaut.Tests.Mock.App;

/// <summary>
/// In-memory stand-in for <see cref="IProjectRepository"/> so service tests stay deterministic.
/// </summary>
internal sealed class MockProjectRepository : IProjectRepository
{
    private readonly Dictionary<Guid, IProject> _projects = new();

    public void Add(IProject project) => _projects[project.Id] = project;

    public Task<IProject?> GetAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(_projects.TryGetValue(id, out var project) ? project : null);

    public Task<IReadOnlyList<IProject>> ListAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<IProject>>(_projects.Values.ToList());

    public Task<IProject> AddAsync(IProject project, CancellationToken ct = default)
    {
        Add(project);
        return Task.FromResult(project);
    }

    public Task UpdateAsync(IProject project, CancellationToken ct = default)
    {
        Add(project);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        _projects.Remove(id);
        return Task.CompletedTask;
    }
}

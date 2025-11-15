using Ergonaut.Core.Models.Project;
namespace Ergonaut.Core.Repositories;

public interface IProjectRepository
{
    Task<IProject?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IProject?> GetByNameAsync(string projectName, CancellationToken ct = default);
    Task<IReadOnlyList<IProject>> ListAsync(CancellationToken ct = default);
    Task<IProject> AddAsync(IProject project, CancellationToken ct = default);
    Task<IProject> UpdateAsync(IProject project, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

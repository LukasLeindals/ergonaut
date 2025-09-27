using Ergonaut.Core.Models.Project;
namespace Ergonaut.Infrastructure.Repositories;

public interface IProjectRepository
{
    Task<IProject?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<IProject>> ListAsync(CancellationToken ct = default);
    Task<IProject> AddAsync(IProject project, CancellationToken ct = default);
    Task UpdateAsync(IProject project, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

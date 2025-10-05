using Ergonaut.Core.Models.Task;

namespace Ergonaut.Infrastructure.Repositories;

public interface ITaskRepository
{
    Task<ITask?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ITask>> ListAsync(CancellationToken ct = default);
    Task<ITask> AddAsync(ITask task, CancellationToken ct = default);
    Task UpdateAsync(ITask task, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

using Ergonaut.Core.Models.Task;
using Ergonaut.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ergonaut.Infrastructure.Repositories;

public sealed class LocalTaskRepository : ITaskRepository
{
    private readonly ErgonautDbContext _db;

    public LocalTaskRepository(ErgonautDbContext db) => _db = db;

    public async Task<ITask?> GetAsync(Guid id, CancellationToken ct = default) =>
        await _db.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);
    public async Task<IReadOnlyList<ITask>> ListAsync(CancellationToken ct = default) =>
        await _db.Tasks.AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyList<ITask>> ListByProjectAsync(Guid projectId, CancellationToken ct = default) =>
        await _db.Tasks.AsNoTracking().Where(t => t.ProjectId == projectId).ToListAsync(ct);
    public async Task<ITask> AddAsync(ITask task, CancellationToken ct = default)
    {
        _db.Tasks.Add(task as LocalTask ?? throw new NotSupportedException($"Expected {nameof(LocalTask)} type but got {task.GetType().Name}."));
        await _db.SaveChangesAsync(ct);
        return task;
    }
    public async Task UpdateAsync(ITask task, CancellationToken ct = default)
    {
        _db.Tasks.Update((LocalTask)task);
        await _db.SaveChangesAsync(ct);
    }
    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Tasks.FindAsync([id], ct);
        if (entity is null)
            return;

        _db.Tasks.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }
}

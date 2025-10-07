using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ergonaut.Core.Models.WorkItem;
using Ergonaut.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ergonaut.Infrastructure.Repositories;

public sealed class WorkItemRepository : IWorkItemRepository
{
    private readonly ErgonautDbContext _db;

    public WorkItemRepository(ErgonautDbContext db) => _db = db;

    public async Task<IWorkItem?> GetAsync(Guid id, CancellationToken ct = default) =>
        await _db.WorkItems.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);
    public async Task<IReadOnlyList<IWorkItem>> ListAsync(CancellationToken ct = default) =>
        await _db.WorkItems.AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyList<IWorkItem>> ListByProjectAsync(Guid projectId, CancellationToken ct = default) =>
        await _db.WorkItems.AsNoTracking().Where(t => t.ProjectId == projectId).ToListAsync(ct);
    public async Task<IWorkItem> AddAsync(IWorkItem workItem, CancellationToken ct = default)
    {
        _db.WorkItems.Add(workItem as WorkItem ?? throw new NotSupportedException($"Expected {nameof(WorkItem)} type but got {workItem.GetType().Name}."));
        await _db.SaveChangesAsync(ct);
        return workItem;
    }
    public async Task UpdateAsync(IWorkItem workItem, CancellationToken ct = default)
    {
        _db.WorkItems.Update((WorkItem)workItem);
        await _db.SaveChangesAsync(ct);
    }
    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.WorkItems.FindAsync([id], ct);
        if (entity is null)
            return;

        _db.WorkItems.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }
}

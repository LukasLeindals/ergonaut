using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ergonaut.Core.Models.WorkItem;

namespace Ergonaut.Infrastructure.Repositories;

public interface IWorkItemRepository
{
    Task<IWorkItem?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<IWorkItem>> ListAsync(CancellationToken ct = default);
    Task<IReadOnlyList<IWorkItem>> ListByProjectAsync(Guid projectId, CancellationToken ct = default);
    Task<IWorkItem> AddAsync(IWorkItem workItem, CancellationToken ct = default);
    Task UpdateAsync(IWorkItem workItem, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

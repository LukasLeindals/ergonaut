using Ergonaut.Core.Models.WorkItem;
using Ergonaut.Core.Repositories;

namespace Ergonaut.Tests.Mock.App;

/// <summary>
/// Minimal in-memory work item store to exercise application services.
/// </summary>
internal sealed class MockWorkItemRepository : IWorkItemRepository
{
    private readonly List<IWorkItem> _workItems = new();

    public IReadOnlyList<IWorkItem> Items => _workItems;

    public void Seed(params IWorkItem[] workItems)
    {
        foreach (var workItem in workItems)
        {
            _workItems.RemoveAll(existing => existing.Id == workItem.Id);
            _workItems.Add(workItem);
        }
    }

    public Task<IWorkItem?> GetAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(_workItems.FirstOrDefault(t => t.Id == id));

    public Task<IReadOnlyList<IWorkItem>> ListAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<IWorkItem>>(_workItems.ToList());

    public Task<IReadOnlyList<IWorkItem>> ListByProjectAsync(Guid projectId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<IWorkItem>>(_workItems.Where(t => t.ProjectId == projectId).ToList());

    public Task<IWorkItem> AddAsync(IWorkItem workItem, CancellationToken ct = default)
    {
        Seed(workItem);
        return Task.FromResult(workItem);
    }

    public Task<IWorkItem> UpdateAsync(IWorkItem workItem, CancellationToken ct = default)
    {
        return AddAsync(workItem, ct);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        _workItems.RemoveAll(workItem => workItem.Id == id);
        return Task.CompletedTask;
    }
}

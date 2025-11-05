using Ergonaut.App.Models;
namespace Ergonaut.App.Sentinel;

public interface ISentinelWorkItemService
{
    Task<WorkItemRecord> CreateAsync(Guid projectId, CreateWorkItemRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<WorkItemRecord>> ListAsync(Guid projectId, CancellationToken ct = default);
}

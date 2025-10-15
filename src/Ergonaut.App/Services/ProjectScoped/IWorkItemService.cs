using Ergonaut.App.Models;
using Ergonaut.Core.Models;

namespace Ergonaut.App.Services.ProjectScoped;

public interface IWorkItemService
{
    Task<IReadOnlyList<WorkItemRecord>> ListAsync(Guid projectId, CancellationToken ct = default);
    Task<WorkItemRecord?> GetAsync(Guid projectId, Guid id, CancellationToken ct = default);
    Task<WorkItemRecord> CreateAsync(Guid projectId, CreateWorkItemRequest request, CancellationToken ct = default);
    Task<DeletionResponse> DeleteAsync(Guid projectId, Guid id, CancellationToken ct = default);
}

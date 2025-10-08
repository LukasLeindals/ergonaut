using Ergonaut.App.Models;

namespace Ergonaut.App.Services;

public interface IWorkItemService
{
    // Lists all work items, ordered by creation date descending.
    Task<IReadOnlyList<WorkItemRecord>> ListAsync(CancellationToken ct = default);

    // Creates a new work item from the given request.
    Task<WorkItemRecord> CreateAsync(CreateWorkItemRequest request, CancellationToken ct = default);

    // Deletes a work item by its ID.
    Task<DeletionResponse> DeleteAsync(Guid id, CancellationToken ct = default);

    // Retrieves a work item by its ID, or null if not found.
    Task<WorkItemRecord?> GetAsync(Guid id, CancellationToken ct = default); // DTO return type
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ergonaut.App.Models;

namespace Ergonaut.App.Features.WorkItems;

public interface IWorkItemService
{
    // Lists all work items, ordered by creation date descending.
    Task<IReadOnlyList<WorkItemSummary>> ListAsync(CancellationToken ct = default);

    // Creates a new work item from the given request.
    Task<WorkItemSummary> CreateAsync(CreateWorkItemRequest request, CancellationToken ct = default);

    // Deletes a work item by its ID.
    Task<DeletionResult> DeleteAsync(Guid id, CancellationToken ct = default);

    // Retrieves a work item by its ID, or null if not found.
    Task<WorkItemSummary?> GetAsync(Guid id, CancellationToken ct = default); // DTO return type
}

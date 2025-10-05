using Ergonaut.App.Models;

namespace Ergonaut.App.Features.Tasks;

public interface ITaskService
{
    // Lists all tasks, ordered by creation date descending.
    Task<IReadOnlyList<TaskSummary>> ListAsync(CancellationToken ct = default);

    // List tasks by project ID.
    Task<IReadOnlyList<TaskSummary>> ListByProjectAsync(Guid projectId, CancellationToken ct = default);

    // Creates a new task from the given request.
    Task<TaskSummary> CreateAsync(Guid projectId, CreateTaskRequest request, CancellationToken ct = default);

    // Deletes a task by its ID.
    Task<DeletionResult> DeleteAsync(Guid id, CancellationToken ct = default);

    // Retrieves a task by its ID, or null if not found.
    Task<TaskSummary?> GetAsync(Guid id, CancellationToken ct = default); // DTO return type
}

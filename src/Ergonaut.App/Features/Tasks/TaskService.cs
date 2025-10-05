using Ergonaut.Infrastructure.Repositories;
using Ergonaut.App.Models;
using Ergonaut.App.Features.Projects;
using Ergonaut.Core.Models.Project;

namespace Ergonaut.App.Features.Tasks;

public sealed class LocalTaskService(ITaskRepository repository, IProjectRepository projectRepository, ITaskFactory taskFactory) : ITaskService
{

    public async Task<IReadOnlyList<TaskSummary>> ListAsync(CancellationToken ct = default) =>
        (await repository.ListAsync(ct))
            .Select(TaskSummary.FromTask)
            .ToArray();

    public async Task<IReadOnlyList<TaskSummary>> ListByProjectAsync(Guid projectId, CancellationToken ct = default) =>
        (await repository.ListByProjectAsync(projectId, ct))
            .Select(TaskSummary.FromTask)
            .ToArray();

    public async Task<TaskSummary> CreateAsync(Guid projectId, CreateTaskRequest request, CancellationToken ct = default)
    {
        IProject? project = await projectRepository.GetAsync(projectId, ct);
        if (project is null)
            throw new InvalidOperationException($"Cannot create a task for project {projectId} since it does not exist.");
        var task = taskFactory.Create(title: request.Title, description: request.Description, projectId: projectId);
        var saved = await repository.AddAsync(task, ct);
        return TaskSummary.FromTask(saved);
    }

    public async Task<DeletionResult> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await repository.GetAsync(id, ct);
        if (existing is null)
            return new DeletionResult(false, "Task not found.");

        await repository.DeleteAsync(id, ct);
        return new DeletionResult(true, "Task deleted successfully.");
    }

    public async Task<TaskSummary?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var task = await repository.GetAsync(id, ct); // repository stays domain-focused
        return task is null ? null : TaskSummary.FromTask(task);
    }
}

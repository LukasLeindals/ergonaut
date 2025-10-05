using Ergonaut.Infrastructure.Repositories;
using Ergonaut.App.Models;
using Ergonaut.App.Features.Projects;
using Ergonaut.Core.Models.Project;
using Ergonaut.Core.Models.Task;

namespace Ergonaut.App.Features.Tasks;

public sealed class LocalProjectScopedTaskService(ITaskRepository repository, IProjectRepository projectRepository) : IProjectScopedTaskService
{

    private Guid? _projectId { get; set; }
    public Guid ProjectId
    {
        get => _projectId ?? throw new InvalidOperationException("ProjectId has not been set.");
        set
        {
            var project = projectRepository.GetAsync(value).GetAwaiter().GetResult();
            if (project is null)
                throw new InvalidOperationException($"Project {value} does not exist.");
            _projectId = value;
        }
    }

    public async Task<IReadOnlyList<TaskSummary>> ListAsync(CancellationToken ct = default)
    {
        return (await repository.ListAsync(ct))
            .Select(TaskSummary.FromTask)
            .Where(task => task.ProjectId == ProjectId)
            .ToArray();
    }

    public async Task<TaskSummary> CreateAsync(CreateTaskRequest request, CancellationToken ct = default)
    {
        LocalTask task = new LocalTask(title: request.Title, description: request.Description, projectId: ProjectId);
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

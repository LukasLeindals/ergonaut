using Microsoft.Extensions.Logging;
using Ergonaut.Core.Repositories;
using Ergonaut.App.Models;
using Ergonaut.Core.Models.WorkItem;

namespace Ergonaut.App.Services;

public sealed class ProjectScopedWorkItemService(IWorkItemRepository repository, IProjectRepository projectRepository, ILogger<ProjectScopedWorkItemService> logger) : IProjectScopedWorkItemService
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

    public async Task<IReadOnlyList<WorkItemRecord>> ListAsync(CancellationToken ct = default)
    {
        return (await repository.ListByProjectAsync(ProjectId, ct))
            .Select(WorkItemRecord.FromWorkItem)
            .OrderByDescending(w => w.CreatedAt)
            .ToArray();
    }

    public async Task<WorkItemRecord> CreateAsync(CreateWorkItemRequest request, CancellationToken ct = default)
    {
        WorkItem workItem = new WorkItem(projectId: ProjectId, title: request.Title, description: request.Description);
        var saved = await repository.AddAsync(workItem, ct);
        return WorkItemRecord.FromWorkItem(saved);
    }

    public async Task<DeletionResponse> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await repository.GetAsync(id, ct);
        if (existing is null)
            return new DeletionResponse(false, "Work item not found.");

        if (existing.ProjectId != ProjectId)
            return new DeletionResponse(false, "Work item does not belong to the scoped project.");

        await repository.DeleteAsync(id, ct);
        return new DeletionResponse(true, "Work item deleted successfully.");
    }

    public async Task<WorkItemRecord?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var workItem = await repository.GetAsync(id, ct); // repository stays domain-focused
        if (workItem is null || workItem.ProjectId != ProjectId)
        {
            logger.LogWarning("Work item {WorkItemId} not found in project {ProjectId}.", id, ProjectId);
            return null;
        }

        return WorkItemRecord.FromWorkItem(workItem);
    }
}

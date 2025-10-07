using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ergonaut.Infrastructure.Repositories;
using Ergonaut.App.Models;
using Ergonaut.App.Features.Projects;
using Ergonaut.Core.Models.Project;
using Ergonaut.Core.Models.WorkItem;

namespace Ergonaut.App.Features.WorkItems;

public sealed class ProjectScopedWorkItemService(IWorkItemRepository repository, IProjectRepository projectRepository) : IProjectScopedWorkItemService
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

    public async Task<IReadOnlyList<WorkItemSummary>> ListAsync(CancellationToken ct = default)
    {
        return (await repository.ListByProjectAsync(ProjectId, ct))
            .Select(WorkItemSummary.FromWorkItem)
            .ToArray();
    }

    public async Task<WorkItemSummary> CreateAsync(CreateWorkItemRequest request, CancellationToken ct = default)
    {
        WorkItem workItem = new WorkItem(projectId: ProjectId, title: request.Title, description: request.Description);
        var saved = await repository.AddAsync(workItem, ct);
        return WorkItemSummary.FromWorkItem(saved);
    }

    public async Task<DeletionResult> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await repository.GetAsync(id, ct);
        if (existing is null)
            return new DeletionResult(false, "Work item not found.");

        if (existing.ProjectId != ProjectId)
            return new DeletionResult(false, "Work item does not belong to the scoped project.");

        await repository.DeleteAsync(id, ct);
        return new DeletionResult(true, "Work item deleted successfully.");
    }

    public async Task<WorkItemSummary?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var workItem = await repository.GetAsync(id, ct); // repository stays domain-focused
        if (workItem is null || workItem.ProjectId != ProjectId)
            return null;

        return WorkItemSummary.FromWorkItem(workItem);
    }
}

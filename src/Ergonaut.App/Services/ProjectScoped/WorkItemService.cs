using System.Linq;
using System.Text.Json;
using Ergonaut.Core.Exceptions;
using Ergonaut.App.Models;
using Ergonaut.Core.Models.WorkItem;
using Ergonaut.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Ergonaut.App.Services.ProjectScoped;

public sealed class WorkItemService(
    IWorkItemRepository repository,
    IProjectRepository projectRepository,
    ILogger<WorkItemService> logger) : IWorkItemService
{
    public async Task<IReadOnlyList<WorkItemRecord>> ListAsync(Guid projectId, CancellationToken ct = default)
    {
        await EnsureProjectExists(projectId, ct);

        var workItems = await repository.ListByProjectAsync(projectId, ct);
        return workItems
            .Select(WorkItemRecord.FromWorkItem)
            .OrderByDescending(w => w.CreatedAt)
            .ToArray();
    }

    public async Task<WorkItemRecord?> GetAsync(Guid projectId, Guid id, CancellationToken ct = default)
    {
        await EnsureProjectExists(projectId, ct);

        var workItem = await repository.GetAsync(id, ct);
        if (workItem is null || workItem.ProjectId != projectId)
        {
            logger.LogWarning("Work item {WorkItemId} not found in project {ProjectId}.", id, projectId);
            return null;
        }

        return WorkItemRecord.FromWorkItem(workItem);
    }

    public async Task<WorkItemRecord> CreateAsync(Guid projectId, CreateWorkItemRequest request, CancellationToken ct = default)
    {
        await EnsureProjectExists(projectId, ct);

        WorkItem workItem = new(
            projectId: projectId, title: request.Title, description: request.Description, sourceLabel: request.SourceLabel,
            status: request.Status, priority: request.Priority, dueDate: request.DueDate, sourceData: request.SourceData ?? new Dictionary<string, JsonElement?>()
        );
        var saved = await repository.AddAsync(workItem, ct);
        return WorkItemRecord.FromWorkItem(saved);
    }

    public async Task<DeletionResponse> DeleteAsync(Guid projectId, Guid id, CancellationToken ct = default)
    {
        await EnsureProjectExists(projectId, ct);

        var existing = await repository.GetAsync(id, ct);
        if (existing is null)
            return new DeletionResponse(false, "Work item not found.");

        if (existing.ProjectId != projectId)
            return new DeletionResponse(false, "Work item does not belong to the specified project.");

        await repository.DeleteAsync(id, ct);
        return new DeletionResponse(true, "Work item deleted successfully.");
    }

    public async Task<WorkItemRecord> UpdateAsync(Guid projectId, Guid id, UpdateWorkItemRequest request, CancellationToken ct = default)
    {
        await EnsureProjectExists(projectId, ct);

        var existing = await repository.GetAsync(id, ct);
        if (existing is null || existing.ProjectId != projectId)
            throw new WorkItemNotFoundException(id, projectId);

        existing.Update(
            title: request.Title,
            description: request.Description,
            status: request.Status,
            priority: request.Priority,
            dueDate: request.DueDate,
            sourceLabel: request.SourceLabel,
            sourceData: request.SourceData
        );

        var updated = await repository.UpdateAsync(existing, ct);
        return WorkItemRecord.FromWorkItem(updated);
    }

    private async Task EnsureProjectExists(Guid projectId, CancellationToken ct)
    {
        if (await projectRepository.GetAsync(projectId, ct) is null)
            throw new ProjectNotFoundException(projectId);
    }
}

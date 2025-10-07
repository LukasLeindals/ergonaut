using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Ergonaut.App.Models;
using Ergonaut.App.Features.WorkItems;

namespace Ergonaut.UI.Features.WorkItems;

internal sealed class ApiWorkItemService(HttpClient client) : IProjectScopedWorkItemService
{
    private Guid? _projectId { get; set; }
    public Guid ProjectId
    {
        get => _projectId ?? throw new InvalidOperationException("ProjectId has not been set.");
        set => _projectId = value;
    }
    public async Task<IReadOnlyList<WorkItemSummary>> ListAsync(CancellationToken ct = default)
    {
        var response = await client.GetAsync($"api/v1/{ProjectId}/work-items", ct);
        response.EnsureSuccessStatusCode();

        var workItems = await response.Content.ReadFromJsonAsync<List<WorkItemSummary>>(cancellationToken: ct);
        return workItems ?? new List<WorkItemSummary>();
    }

    public async Task<WorkItemSummary?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var response = await client.GetAsync($"api/v1/{ProjectId}/work-items/{id:D}", ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<WorkItemSummary>(cancellationToken: ct);
    }

    public async Task<WorkItemSummary> CreateAsync(CreateWorkItemRequest request, CancellationToken ct = default)
    {
        var response = await client.PostAsJsonAsync($"api/v1/{ProjectId}/work-items", request, ct);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<WorkItemSummary>(cancellationToken: ct);
        return created ?? throw new InvalidOperationException("API returned no work item payload.");
    }

    public async Task<DeletionResult> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            HttpResponseMessage response = await client.DeleteAsync($"api/v1/{ProjectId}/work-items/{id:D}", ct);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new DeletionResult(false, "Work item not found.");

            response.EnsureSuccessStatusCode();
            return new DeletionResult(true, "Work item deleted successfully.");
        }
        catch (HttpRequestException ex)
        {
            return new DeletionResult(false, $"Error deleting work item: {ex.Message}");
        }
    }
}

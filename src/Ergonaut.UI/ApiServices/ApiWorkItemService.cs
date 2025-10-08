using Ergonaut.App.Models;
using Ergonaut.App.Services;

namespace Ergonaut.UI.ApiServices;

internal sealed class ApiWorkItemService(HttpClient client) : IProjectScopedWorkItemService
{
    private Guid? _projectId { get; set; }
    public Guid ProjectId
    {
        get => _projectId ?? throw new InvalidOperationException("ProjectId has not been set.");
        set => _projectId = value;
    }
    public async Task<IReadOnlyList<WorkItemRecord>> ListAsync(CancellationToken ct = default)
    {
        var response = await client.GetAsync($"api/v1/{ProjectId}/work-items", ct);
        response.EnsureSuccessStatusCode();

        var workItems = await response.Content.ReadFromJsonAsync<List<WorkItemRecord>>(cancellationToken: ct);
        return workItems ?? new List<WorkItemRecord>();
    }

    public async Task<WorkItemRecord?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var response = await client.GetAsync($"api/v1/{ProjectId}/work-items/{id:D}", ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<WorkItemRecord>(cancellationToken: ct);
    }

    public async Task<WorkItemRecord> CreateAsync(CreateWorkItemRequest request, CancellationToken ct = default)
    {
        var response = await client.PostAsJsonAsync($"api/v1/{ProjectId}/work-items", request, ct);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<WorkItemRecord>(cancellationToken: ct);
        return created ?? throw new InvalidOperationException("API returned no work item payload.");
    }

    public async Task<DeletionResponse> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            HttpResponseMessage response = await client.DeleteAsync($"api/v1/{ProjectId}/work-items/{id:D}", ct);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new DeletionResponse(false, "Work item not found.");

            response.EnsureSuccessStatusCode();
            return new DeletionResponse(true, "Work item deleted successfully.");
        }
        catch (HttpRequestException ex)
        {
            return new DeletionResponse(false, $"Error deleting work item: {ex.Message}");
        }
    }
}

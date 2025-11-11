
using System.Net;
using System.Net.Http.Json;
using Ergonaut.App.Models;
using Ergonaut.App.Services.ProjectScoped;

namespace Ergonaut.App.Services.ApiScoped;

public sealed class ApiWorkItemService(HttpClient client) : IWorkItemService
{
    public async Task<IReadOnlyList<WorkItemRecord>> ListAsync(Guid projectId, CancellationToken ct = default)
    {
        var response = await client.GetAsync($"api/v1/{projectId}/work-items", ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new InvalidOperationException($"Project {projectId} was not found.");

        response.EnsureSuccessStatusCode();

        var workItems = await response.Content.ReadFromJsonAsync<List<WorkItemRecord>>(cancellationToken: ct);
        return workItems ?? new List<WorkItemRecord>();
    }

    public async Task<WorkItemRecord?> GetAsync(Guid projectId, Guid id, CancellationToken ct = default)
    {
        var response = await client.GetAsync($"api/v1/{projectId}/work-items/{id:D}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<WorkItemRecord>(cancellationToken: ct);
    }

    public async Task<WorkItemRecord> CreateAsync(Guid projectId, CreateWorkItemRequest request, CancellationToken ct = default)
    {
        var response = await client.PostAsJsonAsync($"api/v1/{projectId}/work-items", request, ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new InvalidOperationException($"Project {projectId} was not found.");

        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<WorkItemRecord>(cancellationToken: ct);
        return created ?? throw new InvalidOperationException("API returned no work item payload.");
    }

    public async Task<DeletionResponse> DeleteAsync(Guid projectId, Guid id, CancellationToken ct = default)
    {
        try
        {
            HttpResponseMessage response = await client.DeleteAsync($"api/v1/{projectId}/work-items/{id:D}", ct);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new DeletionResponse(false, "Work item not found.");
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var reason = await response.Content.ReadAsStringAsync(ct);
                return new DeletionResponse(false, string.IsNullOrWhiteSpace(reason) ? "Failed to delete work item." : reason);
            }

            response.EnsureSuccessStatusCode();
            return new DeletionResponse(true, "Work item deleted successfully.");
        }
        catch (HttpRequestException ex)
        {
            return new DeletionResponse(false, $"Error deleting work item: {ex.Message}");
        }
    }
}

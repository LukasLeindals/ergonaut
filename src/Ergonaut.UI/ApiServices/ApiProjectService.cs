using Ergonaut.App.Models;
using Ergonaut.App.Services;

namespace Ergonaut.UI.ApiServices;

internal sealed class ApiProjectService(HttpClient client) : IProjectService
{
    public async Task<IReadOnlyList<ProjectRecord>> ListAsync(CancellationToken ct = default)
    {
        var response = await client.GetAsync("api/v1/projects", ct);
        response.EnsureSuccessStatusCode();

        var projects = await response.Content.ReadFromJsonAsync<List<ProjectRecord>>(cancellationToken: ct);
        return projects ?? new List<ProjectRecord>();
    }

    public async Task<ProjectRecord?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var response = await client.GetAsync($"api/v1/projects/{id:D}", ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProjectRecord>(cancellationToken: ct);
    }

    public async Task<ProjectRecord> CreateAsync(CreateProjectRequest request, CancellationToken ct = default)
    {
        var response = await client.PostAsJsonAsync("api/v1/projects", request, ct);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<ProjectRecord>(cancellationToken: ct);
        return created ?? throw new InvalidOperationException("API returned no project payload.");
    }

    public async Task<DeletionResponse> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            HttpResponseMessage response = await client.DeleteAsync($"api/v1/projects/{id:D}", ct);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new DeletionResponse(false, "Project not found.");

            response.EnsureSuccessStatusCode();
            return new DeletionResponse(true, "Project deleted successfully.");
        }
        catch (HttpRequestException ex)
        {
            return new DeletionResponse(false, $"Error deleting project: {ex.Message}");
        }
    }
}

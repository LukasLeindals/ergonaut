using Ergonaut.App.Models;
using Ergonaut.App.Features.Projects;

namespace Ergonaut.UI.Features.Projects;

internal sealed class ApiProjectService(HttpClient client) : IProjectService
{
    public async Task<IReadOnlyList<ProjectSummary>> ListAsync(CancellationToken ct = default)
    {
        var response = await client.GetAsync("api/v1/projects", ct);
        response.EnsureSuccessStatusCode();

        var projects = await response.Content.ReadFromJsonAsync<List<ProjectSummary>>(cancellationToken: ct);
        return projects ?? new List<ProjectSummary>();
    }

    public async Task<ProjectSummary?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var response = await client.GetAsync($"api/v1/projects/{id:D}", ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProjectSummary>(cancellationToken: ct);
    }

    public async Task<ProjectSummary> CreateAsync(CreateProjectRequest request, CancellationToken ct = default)
    {
        var response = await client.PostAsJsonAsync("api/v1/projects", request, ct);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<ProjectSummary>(cancellationToken: ct);
        return created ?? throw new InvalidOperationException("API returned no project payload.");
    }

    public async Task<DeletionResult> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            HttpResponseMessage response = await client.DeleteAsync($"api/v1/projects/{id:D}", ct);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new DeletionResult(false, "Project not found.");

            response.EnsureSuccessStatusCode();
            return new DeletionResult(true, "Project deleted successfully.");
        }
        catch (HttpRequestException ex)
        {
            return new DeletionResult(false, $"Error deleting project: {ex.Message}");
        }
    }
}

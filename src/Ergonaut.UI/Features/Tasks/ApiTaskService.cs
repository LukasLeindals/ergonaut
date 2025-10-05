using Ergonaut.App.Models;
using Ergonaut.App.Features.Projects;
using Ergonaut.App.Features.Tasks;

namespace Ergonaut.UI.Features.Tasks;

internal sealed class ApiTaskService(HttpClient client) : ITaskService
{
    public async Task<IReadOnlyList<TaskSummary>> ListAsync(CancellationToken ct = default)
    {
        var response = await client.GetAsync("api/v1/tasks", ct);
        response.EnsureSuccessStatusCode();
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskSummary>>(cancellationToken: ct);
        return tasks ?? new List<TaskSummary>();
    }
    public async Task<IReadOnlyList<TaskSummary>> ListByProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        var response = await client.GetAsync($"api/v1/{projectId}/tasks", ct);
        response.EnsureSuccessStatusCode();

        var tasks = await response.Content.ReadFromJsonAsync<List<TaskSummary>>(cancellationToken: ct);
        return tasks ?? new List<TaskSummary>();
    }

    public async Task<TaskSummary?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var response = await client.GetAsync($"api/v1/tasks/{id:D}", ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TaskSummary>(cancellationToken: ct);
    }

    public async Task<TaskSummary> CreateAsync(Guid projectId, CreateTaskRequest request, CancellationToken ct = default)
    {
        var response = await client.PostAsJsonAsync($"api/v1/{projectId}/tasks", request, ct);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<TaskSummary>(cancellationToken: ct);
        return created ?? throw new InvalidOperationException("API returned no task payload.");
    }

    public async Task<DeletionResult> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            HttpResponseMessage response = await client.DeleteAsync($"api/v1/tasks/{id:D}", ct);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new DeletionResult(false, "Task not found.");

            response.EnsureSuccessStatusCode();
            return new DeletionResult(true, "Task deleted successfully.");
        }
        catch (HttpRequestException ex)
        {
            return new DeletionResult(false, $"Error deleting task: {ex.Message}");
        }
    }
}

namespace Ergonaut.UI.Components.Pages.WorkItems;

public partial class WorkItems
{
    private async Task LoadProjectsAsync()
    {
        Logger.LogInformation("Loading projects...");
        try
        {
            _errorMessage = null;
            _projects = (await projectApi.ListAsync(CancellationToken.None))
                .OrderBy(p => p.Title, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load projects");
            _errorMessage = "We couldn’t load your projects. Please retry.";
        }
    }

    private async Task LoadWorkItemsAsync()
    {
        if (SelectedProjectId is not Guid projectId)
        {
            Logger.LogInformation("No project selected; skipping work item load.");
            _workItems = null;
            return;
        }

        Logger.LogInformation("Loading work items for project {ProjectId}", projectId);
        try
        {
            _isLoadingWorkItems = true;
            _errorMessage = null;
            _workItems = (await _workItemApi.ListAsync(projectId, CancellationToken.None)).ToList();
            Logger.LogInformation("Loaded {WorkItemCount} work items for project {ProjectId}", _workItems.Count, projectId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load work items for project {ProjectId}", projectId);
            _errorMessage = "We couldn’t load work items for this project. Please retry.";
        }
        finally
        {
            Logger.LogInformation("Finished loading work items for project {ProjectId}", projectId);
            _isLoadingWorkItems = false;
        }
    }
}

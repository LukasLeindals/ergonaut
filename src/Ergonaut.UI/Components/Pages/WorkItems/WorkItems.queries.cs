using System.Threading;

namespace Ergonaut.UI.Components.Pages.WorkItems;

public partial class WorkItems
{
    private async Task LoadProjectsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading projects...");
        try
        {
            _errorMessage = null;
            _projects = (await _projectApi.ListAsync(cancellationToken))
                .OrderBy(p => p.Title, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load projects");
            _errorMessage = "We couldn’t load your projects. Please retry.";
        }
    }

    private async Task LoadWorkItemsAsync(CancellationToken cancellationToken)
    {
        if (SelectedProjectId is not Guid projectId)
        {
            _logger.LogInformation("No project selected; skipping work item load.");
            _workItems = null;
            return;
        }

        _logger.LogInformation("Loading work items for project {ProjectId}", projectId);
        try
        {
            _isLoadingWorkItems = true;
            _errorMessage = null;
            _workItems = (await _workItemApi.ListAsync(projectId, cancellationToken)).ToList();
            _logger.LogInformation("Loaded {WorkItemCount} work items for project {ProjectId}", _workItems.Count, projectId);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Work item load for project {ProjectId} canceled.", projectId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load work items for project {ProjectId}", projectId);
            _errorMessage = "We couldn’t load work items for this project. Please retry.";
        }
        finally
        {
            _logger.LogInformation("Finished loading work items for project {ProjectId}", projectId);
            _isLoadingWorkItems = false;
        }
    }
}

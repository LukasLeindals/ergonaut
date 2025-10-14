using System;
using Ergonaut.App.Models;
using Ergonaut.Core.Models.WorkItem;

namespace Ergonaut.UI.Components.Pages.WorkItems;

public partial class WorkItems
{
    private async Task CreateWorkItemAsync()
    {
        if (_isSubmitting)
            return;

        if (SelectedProjectId is not Guid projectId)
        {
            _errorMessage = "Select a project before creating a work item.";
            return;
        }

        _isSubmitting = true;
        _errorMessage = null;

        Logger.LogInformation("Creating work item {WorkItemTitle} for project {ProjectId}", _workItemForm.Title, projectId);

        try
        {
            WorkItemRecord created = await _workItemApi.CreateAsync(projectId, _workItemForm, CancellationToken.None);
            _workItems ??= new();
            _workItems.Insert(0, created);
            ResetWorkItemForm();
            HideCreateModal();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to create work item");
            _errorMessage = "We couldn’t create the work item. Check the details and try again.";
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private async Task DeleteWorkItemAsync(WorkItemRecord workItem)
    {
        if (_isSubmitting)
            return;

        if (SelectedProjectId is not Guid projectId)
        {
            _errorMessage = "Select a project before deleting work items.";
            return;
        }

        _isSubmitting = true;
        _errorMessage = null;

        Logger.LogInformation("Deleting work item {WorkItemId} from project {ProjectId}", workItem.Id, workItem.ProjectId);

        try
        {
            DeletionResponse deletionResponse = await _workItemApi.DeleteAsync(projectId, workItem.Id, CancellationToken.None);
            if (deletionResponse.Success)
            {
                _workItems?.Remove(workItem);
            }
            else
            {
                Logger.LogWarning("Failed to delete work item: {Reason}", deletionResponse.Message);
                _errorMessage = deletionResponse.Message;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to delete work item {WorkItemId}", workItem.Id);
            _errorMessage = "We couldn’t delete the work item. Please try again.";
        }
        finally
        {
            _isSubmitting = false;
        }
    }
}

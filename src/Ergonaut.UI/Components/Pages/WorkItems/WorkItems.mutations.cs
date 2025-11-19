using System;
using System.Threading;
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

        _logger.LogInformation("Creating work item {WorkItemTitle} for project {ProjectId}", _workItemForm.Title, projectId);

        try
        {
            WorkItemRecord created = await _workItemApi.CreateAsync(projectId, _workItemForm, ComponentToken);
            _workItems ??= new();
            _workItems.Insert(0, created);
            ResetWorkItemForm();
            HideCreateModal();
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Work item creation canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create work item");
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

        _logger.LogInformation("Deleting work item {WorkItemId} from project {ProjectId}", workItem.Id, workItem.ProjectId);

        try
        {
            DeletionResponse deletionResponse = await _workItemApi.DeleteAsync(projectId, workItem.Id, ComponentToken);
            if (deletionResponse.Success)
            {
                _workItems?.Remove(workItem);
            }
            else
            {
                _logger.LogWarning("Failed to delete work item: {Reason}", deletionResponse.Message);
                _errorMessage = deletionResponse.Message;
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Work item deletion canceled for {WorkItemId}.", workItem.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete work item {WorkItemId}", workItem.Id);
            _errorMessage = "We couldn’t delete the work item. Please try again.";
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private async Task UpdateWorkItemAsync()
    {
        if (_isSubmitting)
            return;

        if (SelectedProjectId is not Guid projectId)
        {
            _errorMessage = "Select a project before updating work items.";
            return;
        }

        if (_selectedWorkItem is null || _editedWorkItem is null)
        {
            _errorMessage = "No work item selected for update.";
            return;
        }

        _isSubmitting = true;
        _errorMessage = null;

        _logger.LogInformation("Updating work item {WorkItemId} in project {ProjectId}", _selectedWorkItem.Id, projectId);

        try
        {
            WorkItemRecord updated = await _workItemApi.UpdateAsync(projectId, _selectedWorkItem.Id, _editedWorkItem, ComponentToken);
            int index = _workItems?.FindIndex(w => w.Id == updated.Id) ?? -1;
            if (index >= 0)
            {
                _workItems![index] = updated;
            }
            HideDetailsModal();
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Work item update canceled for {WorkItemId}.", _selectedWorkItem.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update work item {WorkItemId}", _selectedWorkItem.Id);
            _errorMessage = "We couldn’t update the work item. Check the details and try again.";
        }
        finally
        {
            _isSubmitting = false;
        }
    }
}

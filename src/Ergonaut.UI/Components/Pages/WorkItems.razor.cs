using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Ergonaut.App.Models;
using Ergonaut.App.Services;
using Ergonaut.UI.ApiServices;
using Ergonaut.Core.Models.Project;
using Ergonaut.Core.Models.WorkItem;

namespace Ergonaut.UI.Components.Pages;

public partial class WorkItems : ComponentBase
{
    [Inject] private IProjectService projectApi { get; set; } = default!;
    [Inject] private IProjectScopedWorkItemService _workItemApi { get; set; } = default!;
    [Inject] private ILogger<WorkItems> Logger { get; set; } = default!;

    private List<ProjectRecord>? _projects;
    private List<WorkItemRecord>? _workItems;
    private CreateWorkItemRequest _workItemForm = new();
    private Guid? _selectedProjectId;

    private bool _isSubmitting;
    private bool _isLoadingWorkItems;
    private string? _errorMessage;
    private bool _showCreateModal;

    protected override async Task OnInitializedAsync()
    {
        await LoadProjectsAsync();
        await LoadWorkItemsAsync();
    }

    private IProjectScopedWorkItemService WorkItemApi
    {
        get
        {
            if (SelectedProjectId is not Guid projectId)
                throw new InvalidOperationException("No project selected; cannot access work items.");

            if (_workItemApi is not IProjectScopedWorkItemService scoped)
                throw new InvalidOperationException("Injected work item service does not support project scoping.");

            scoped.ProjectId = projectId;
            return scoped;
        }
    }

    private Guid? SelectedProjectId
    {
        get => _selectedProjectId;
        set
        {
            if (_selectedProjectId != value)
            {
                _selectedProjectId = value;
                _ = InvokeAsync(async () => { await LoadWorkItemsAsync(); StateHasChanged(); });
            }
        }
    }

    private void ResetWorkItemForm() => _workItemForm = new();

    private async Task LoadProjectsAsync()
    {
        Logger.LogInformation("Loading projects...");
        try
        {
            _errorMessage = null;
            _projects = (await projectApi.ListAsync(CancellationToken.None)).OrderBy(p => p.Title).ToList();
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
            _workItems = (await WorkItemApi.ListAsync(CancellationToken.None)).ToList();
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
            WorkItemRecord created = await WorkItemApi.CreateAsync(_workItemForm, CancellationToken.None);
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

        _isSubmitting = true;
        _errorMessage = null;

        Logger.LogInformation("Deleting work item {WorkItemId} from project {ProjectId}", workItem.Id, workItem.ProjectId);

        try
        {
            DeletionResponse deletionResponse = await WorkItemApi.DeleteAsync(workItem.Id, CancellationToken.None);
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

    private void HandleInvalidSubmit(EditContext context)
    {
        Logger.LogInformation("Invalid work item submission: {Title}", _workItemForm.Title);
        Logger.LogWarning("Invalid submit; errors: {Errors}", string.Join(", ", context.GetValidationMessages()));
    }

    private void ShowCreateModal()
    {
        if (SelectedProjectId is null)
        {
            _errorMessage = "Select a project before creating work items.";
            return;
        }

        ResetWorkItemForm();
        _errorMessage = null;
        _showCreateModal = true;
    }

    private void HideCreateModal() => _showCreateModal = false;
}

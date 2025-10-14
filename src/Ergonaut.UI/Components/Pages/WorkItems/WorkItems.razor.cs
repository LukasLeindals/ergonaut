using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Ergonaut.App.Models;
using Ergonaut.App.Services;
using Ergonaut.App.Services.ProjectScoped;

namespace Ergonaut.UI.Components.Pages.WorkItems;

public partial class WorkItems : ComponentBase
{
    [Inject] private IProjectService projectApi { get; set; } = default!;
    [Inject] private IWorkItemService _workItemApi { get; set; } = default!;
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

    private Guid? SelectedProjectId
    {
        get => _selectedProjectId;
        set
        {
            if (_selectedProjectId != value)
            {
                _selectedProjectId = value;
                _ = InvokeAsync(async () =>
                {
                    await LoadWorkItemsAsync();
                    StateHasChanged();
                });
            }
        }
    }

    private void SetSelectedProject(Guid? projectId) => SelectedProjectId = projectId;

    private void ResetWorkItemForm() => _workItemForm = new();

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

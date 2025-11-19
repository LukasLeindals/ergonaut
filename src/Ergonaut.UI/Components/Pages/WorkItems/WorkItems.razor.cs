using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Ergonaut.Core.Models;
using Ergonaut.App.Models;
using Ergonaut.App.Services;
using Ergonaut.App.Services.ProjectScoped;

using System.Threading;

namespace Ergonaut.UI.Components.Pages.WorkItems;

public partial class WorkItems : ComponentBase, IDisposable
{
    [Inject] private IProjectService _projectApi { get; set; } = default!;
    [Inject] private IWorkItemService _workItemApi { get; set; } = default!;
    [Inject] private ILogger<WorkItems> _logger { get; set; } = default!;

    private List<ProjectRecord>? _projects;
    private List<WorkItemRecord>? _workItems;
    private CreateWorkItemRequest _workItemForm = new(sourceLabel: SourceLabel.Ergonaut);
    private Guid? _selectedProjectId;
    private WorkItemRecord? _selectedWorkItem;
    private UpdateWorkItemRequest? _editedWorkItem;

    private readonly CancellationTokenSource _componentCts = new();
    private CancellationToken ComponentToken => _componentCts.Token;

    private bool _isSubmitting;
    private bool _isLoadingWorkItems;
    private string? _errorMessage;
    private bool _showCreateModal;
    private bool _showDetailsModal;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await LoadProjectsAsync(ComponentToken);
            await LoadWorkItemsAsync(ComponentToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Component initialization canceled.");
        }
    }

    private Guid? SelectedProjectId => _selectedProjectId;

    private async Task SetSelectedProject(Guid? projectId)
    {
        if (_selectedProjectId == projectId)
        {
            return;
        }

        _selectedProjectId = projectId;

        try
        {
            await LoadWorkItemsAsync(ComponentToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Project selection change canceled.");
        }
    }

    private void ResetWorkItemForm() => _workItemForm = new();

    private void HandleInvalidSubmit(EditContext context)
    {
        _logger.LogInformation("Invalid work item submission: {Title}", _workItemForm.Title);
        _logger.LogWarning("Invalid submit; errors: {Errors}", string.Join(", ", context.GetValidationMessages()));
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

    private void ShowDetailsModal(WorkItemRecord workItem)
    {
        _selectedWorkItem = workItem;
        _editedWorkItem = UpdateWorkItemRequest.FromWorkItem(workItem);
        _showDetailsModal = true;
    }
    private void HideDetailsModal()
    {
        _selectedWorkItem = null;
        _editedWorkItem = null;
        _showDetailsModal = false;
    }
    public void Dispose()
    {
        if (!_componentCts.IsCancellationRequested)
        {
            _componentCts.Cancel();
        }

        _componentCts.Dispose();
    }
}

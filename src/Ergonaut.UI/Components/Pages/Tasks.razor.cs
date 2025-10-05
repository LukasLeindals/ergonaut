using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Ergonaut.App.Models;
using Ergonaut.App.Features.Projects;
using Ergonaut.UI.Features.Projects;
using Ergonaut.App.Features.Tasks;
using Ergonaut.UI.Features.Tasks;

namespace Ergonaut.UI.Components.Pages;

public partial class Tasks : ComponentBase
{
    [Inject] private IProjectService projectApi { get; set; } = default!;
    [Inject] private IProjectScopedTaskService _taskApi { get; set; } = default!;
    [Inject] private ILogger<Tasks> Logger { get; set; } = default!;

    private List<ProjectInfo>? _projects;
    private List<TaskSummary>? _tasks;
    private CreateTaskRequest _taskForm = new();
    private Guid? _selectedProjectId;

    private bool _isSubmitting;
    private bool _isLoadingTasks;
    private string? _errorMessage;
    private bool _showCreateModal;


    protected override async Task OnInitializedAsync()
    {
        await LoadProjectsAsync();
        await LoadTasksAsync();
    }

    private IProjectScopedTaskService taskApi
    {
        get
        {
            if (SelectedProjectId is not Guid projectId)
                throw new InvalidOperationException("No project selected; cannot access tasks.");

            if (_taskApi is not IProjectScopedTaskService scoped)
                throw new InvalidOperationException("Injected task service does not support project scoping.");

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
                _ = InvokeAsync(async () => { await LoadTasksAsync(); StateHasChanged(); });
            }
        }
    }

    private static ProjectInfo ToInfo(ProjectSummary project) =>
        new(project.Id, project.Title, project.CreatedAt);

    private void ResetTaskForm() => _taskForm = new();

    private async Task LoadProjectsAsync()
    {
        Logger.LogInformation("Loading projects...");
        try
        {
            _errorMessage = null;
            _projects = (await projectApi.ListAsync(CancellationToken.None)).Select(ToInfo).OrderBy(p => p.Title).ToList();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load projects");
            _errorMessage = "We couldn’t load your projects. Please retry.";
        }
    }

    private async Task LoadTasksAsync()
    {
        if (SelectedProjectId is not Guid projectId)
        {
            Logger.LogInformation("No project selected; skipping task load.");
            _tasks = null;
            return;
        }
        Logger.LogInformation("Loading tasks for project {ProjectId}", projectId);
        try
        {
            _isLoadingTasks = true;
            _errorMessage = null;
            _tasks = (await taskApi.ListAsync(CancellationToken.None)).ToList();
            Logger.LogInformation("Loaded {TaskCount} tasks for project {ProjectId}", _tasks.Count, projectId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load tasks for project {ProjectId}", projectId);
            _errorMessage = "We couldn’t load tasks for this project. Please retry.";
        }
        finally
        {
            Logger.LogInformation("Finished loading tasks for project {ProjectId}", projectId);
            _isLoadingTasks = false;
        }
    }

    private async Task CreateTaskAsync()
    {
        if (_isSubmitting)
            return;

        if (SelectedProjectId is not Guid projectId)
        {
            _errorMessage = "Select a project before creating a task.";
            return;
        }

        _isSubmitting = true;
        _errorMessage = null;

        Logger.LogInformation("Creating task {TaskTitle} for project {ProjectId}", _taskForm.Title, projectId);

        try
        {
            TaskSummary created = await taskApi.CreateAsync(_taskForm, CancellationToken.None);
            _tasks ??= new();
            _tasks.Insert(0, created);
            ResetTaskForm();
            HideCreateModal();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to create task");
            _errorMessage = "We couldn’t create the task. Check the details and try again.";
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private async Task DeleteTaskAsync(TaskSummary task)
    {
        if (_isSubmitting)
            return;

        _isSubmitting = true;
        _errorMessage = null;

        Logger.LogInformation("Deleting task {TaskId} from project {ProjectId}", task.Id, task.ProjectId);

        try
        {
            DeletionResult result = await taskApi.DeleteAsync(task.Id, CancellationToken.None);
            if (result.Success)
            {
                _tasks?.Remove(task);
            }
            else
            {
                Logger.LogWarning("Failed to delete task: {Reason}", result.Message);
                _errorMessage = result.Message ?? "We couldn’t delete the task. Please try again.";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to delete task {TaskId}", task.Id);
            _errorMessage = "We couldn’t delete the task. Please try again.";
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private void HandleInvalidSubmit(EditContext context)
    {
        Logger.LogInformation("Invalid task submission: {Title}", _taskForm.Title);
        Logger.LogWarning("Invalid submit; errors: {Errors}", string.Join(", ", context.GetValidationMessages()));
    }

    private void ShowCreateModal()
    {
        if (SelectedProjectId is null)
        {
            _errorMessage = "Select a project before creating tasks.";
            return;
        }

        ResetTaskForm();
        _errorMessage = null;
        _showCreateModal = true;
    }

    private void HideCreateModal() => _showCreateModal = false;
}

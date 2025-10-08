using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Ergonaut.App.Models;
using Ergonaut.App.Services;

namespace Ergonaut.UI.Components.Pages;

public partial class Projects : ComponentBase
{
    [Inject] private IProjectService projectApi { get; set; } = default!;
    [Inject] private ILogger<Projects> Logger { get; set; } = default!;

    private CreateProjectRequest _form = new();
    private List<ProjectRecord>? _projects;
    private bool _isSubmitting;
    private string? _errorMessage;
    private bool _showCreateModal;

    protected override async Task OnInitializedAsync()
    {
        ResetForm();
        await LoadProjectsAsync();
    }

    private void ResetForm()
    {
        _form = new CreateProjectRequest();
    }

    private async Task LoadProjectsAsync()
    {
        Logger.LogInformation("Loading projects...");
        try
        {
            _projects = (await projectApi.ListAsync(CancellationToken.None)).ToList();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load projects");
            _errorMessage = "We couldn’t load your projects. Please retry.";
        }
    }

    private async Task CreateProjectAsync()
    {
        if (_isSubmitting)
            return;
        _isSubmitting = true;
        _errorMessage = null;

        Logger.LogInformation("Creating project {ProjectTitle}", _form.Title);

        try
        {
            ProjectRecord created = await projectApi.CreateAsync(_form, CancellationToken.None);
            _projects ??= new();
            _projects.Insert(0, created);
            ResetForm(); // clears model + validation state
            HideCreateModal();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to create project");
            _errorMessage = "We couldn’t create the project. Double-check the name and try again.";
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private async Task DeleteProjectAsync(ProjectRecord project)
    {
        if (_isSubmitting)
            return;
        _isSubmitting = true;
        _errorMessage = null;

        Logger.LogInformation("Deleting project {ProjectId} ({ProjectTitle})", project.Id, project.Title);

        try
        {
            DeletionResponse deletionResponse = await projectApi.DeleteAsync(project.Id, CancellationToken.None);
            if (deletionResponse.Success)
            {
                _projects?.Remove(project);
            }
            else
            {
                Logger.LogWarning("Failed to delete project: {Reason}", deletionResponse.Message);
                _errorMessage = "We couldn’t delete the project. Please try again.";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to delete project");
            _errorMessage = "We couldn’t delete the project. Please try again.";
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private void HandleInvalidSubmit(EditContext context)
    {
        Logger.LogInformation($"Invalid form submission: {_form.Title}");
        Logger.LogWarning("Invalid submit; errors: {Errors}", string.Join(", ", context.GetValidationMessages()));
    }

    private void ShowCreateModal()
    {
        ResetForm(); // fresh form each time
        _errorMessage = null;
        _showCreateModal = true;
    }

    private void HideCreateModal()
    {
        _showCreateModal = false;
    }
}

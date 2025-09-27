using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Ergonaut.App.Features.Projects;
using Ergonaut.UI.Features.Projects;

namespace Ergonaut.UI.Components.Pages;

public partial class Projects : ComponentBase
{
    [Inject] private ProjectInfoQuery _projectInfoQuery { get; set; } = default!;
    [Inject] private ILogger<Projects> Logger { get; set; } = default!;

    private CreateProjectRequest _form = new();
    private List<ProjectInfo>? _projects;
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
            _projects = (await _projectInfoQuery.ListAsync(CancellationToken.None)).ToList();
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
            var created = await _projectInfoQuery.CreateAsync(_form, CancellationToken.None);
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

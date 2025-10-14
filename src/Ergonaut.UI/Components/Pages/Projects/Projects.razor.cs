using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Ergonaut.App.Models;
using Ergonaut.App.Services;

namespace Ergonaut.UI.Components.Pages.Projects;

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

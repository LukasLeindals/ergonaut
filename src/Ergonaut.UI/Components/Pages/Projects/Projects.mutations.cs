using Ergonaut.App.Models;

namespace Ergonaut.UI.Components.Pages.Projects;

public partial class Projects
{
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
}

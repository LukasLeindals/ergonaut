namespace Ergonaut.UI.Components.Pages.Projects;

public partial class Projects
{
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
}

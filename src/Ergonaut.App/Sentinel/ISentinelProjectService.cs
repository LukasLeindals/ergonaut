using Ergonaut.App.Models;
namespace Ergonaut.App.Sentinel;

public interface ISentinelProjectService
{
    Task<ProjectRecord?> GetProjectByName(string projectName, CancellationToken ct = default);

    Task<ProjectRecord> CreateAsync(CreateProjectRequest request, CancellationToken ct = default);
}

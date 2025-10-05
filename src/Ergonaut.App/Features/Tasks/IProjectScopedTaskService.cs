using Ergonaut.App.Models;

namespace Ergonaut.App.Features.Tasks;

public interface IProjectScopedTaskService : ITaskService
{
    // Get a valid project ID or throw if not set.
    Guid ProjectId { get; set; }

    IProjectScopedTaskService UseProject(Guid projectId)
    {
        ProjectId = projectId;
        return this;
    }
}

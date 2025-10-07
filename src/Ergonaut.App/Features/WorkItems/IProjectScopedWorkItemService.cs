using System;
using Ergonaut.App.Models;

namespace Ergonaut.App.Features.WorkItems;

public interface IProjectScopedWorkItemService : IWorkItemService
{
    // Get a valid project ID or throw if not set.
    Guid ProjectId { get; set; }

    IProjectScopedWorkItemService UseProject(Guid projectId)
    {
        ProjectId = projectId;
        return this;
    }
}

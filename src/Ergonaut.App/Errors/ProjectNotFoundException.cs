using System;

namespace Ergonaut.App.Errors;

public sealed class ProjectNotFoundException : InvalidOperationException
{
    public ProjectNotFoundException(Guid projectId)
        : base($"Project {projectId} does not exist.")
    {
        ProjectId = projectId;
    }

    public Guid ProjectId { get; }
}

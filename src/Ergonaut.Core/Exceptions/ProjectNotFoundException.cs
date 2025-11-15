namespace Ergonaut.Core.Exceptions;

public class ProjectNotFoundException : Exception
{
    public ProjectNotFoundException(Guid projectId)
        : base($"Project with ID '{projectId}' was not found.")
    {
    }
}

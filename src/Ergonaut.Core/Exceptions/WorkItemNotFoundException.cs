namespace Ergonaut.Core.Exceptions;

public class WorkItemNotFoundException : Exception
{
    public WorkItemNotFoundException(Guid workItemId, Guid? projectId = null)
        : base(BuildMessage(workItemId, projectId))
    {
    }

    private static string BuildMessage(Guid workItemId, Guid? projectId)
    {
        if (projectId.HasValue)
        {
            return $"Work item with ID '{workItemId}' was not found in project '{projectId}'.";
        }
        else
        {
            return $"Work item with ID '{workItemId}' was not found.";
        }
    }
}

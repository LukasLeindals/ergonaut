using Ergonaut.Core.Models.Task;

namespace Ergonaut.App.Features.Tasks;

public sealed class LocalTaskFactory : ITaskFactory
{
    public ITask Create(Guid projectId, string title, string? description) => new LocalTask(title: title, description: description, projectId: projectId);
}

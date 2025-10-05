using Ergonaut.Core.Models.Task;

namespace Ergonaut.App.Features.Tasks;

public interface ITaskFactory
{
    ITask Create(Guid projectId, string title, string? description = null);
}

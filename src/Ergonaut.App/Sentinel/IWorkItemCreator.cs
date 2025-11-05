using Ergonaut.Core.LogIngestion;
namespace Ergonaut.App.Sentinel;

public interface IWorkItemCreator
{
    /// <summary>
    /// Creates a work item for the given log event.
    /// </summary>
    /// <param name="logEvent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CreateWorkItem(ILogEvent logEvent, CancellationToken cancellationToken = default);
}

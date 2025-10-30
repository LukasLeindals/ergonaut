using System.Threading;
using Ergonaut.Core.LogIngestion;

namespace Ergonaut.App.Sentinel;

public interface ILogEventFilter
{
    bool Accept(ILogEvent logEvent, CancellationToken cancellationToken = default);
}

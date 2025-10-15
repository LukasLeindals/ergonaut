using System.Diagnostics;

namespace Ergonaut.App.Sentinel;

public interface IProcessLogHandler
{
    void HandleOutputData(object? sender, DataReceivedEventArgs e);
    void HandleErrorData(object? sender, DataReceivedEventArgs e);
}

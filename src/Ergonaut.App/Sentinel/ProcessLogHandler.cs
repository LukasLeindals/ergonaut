using System.Diagnostics;

namespace Ergonaut.App.Sentinel;

public class ProcessLogHandler : IProcessLogHandler
{
    public void HandleOutputData(object? sender, DataReceivedEventArgs e)
    {
        if (e.Data is not null)
        {
            Console.WriteLine($"Output data received: {e.Data}");
        }
    }

    public void HandleErrorData(object? sender, DataReceivedEventArgs e)
    {
        if (e.Data is not null)
        {
            Console.WriteLine($"Error data received: {e.Data}");
        }
    }
}

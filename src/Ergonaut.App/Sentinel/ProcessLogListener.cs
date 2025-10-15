using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Ergonaut.Core.Models.Logging;

namespace Ergonaut.App.Sentinel;

/// <summary>
/// Attaches to an externally managed process, adapts its output to log events, and publishes to the log hub.
/// </summary>
public sealed class ProcessLogListener : IAsyncDisposable
{
    private readonly IProcessLogHandler _logEventHandler;

    private CancellationTokenSource? _cts;
    private Task? _pumpTask;
    private Process? _process;

    public ProcessLogListener(string name, LogEventHub hub, IProcessLogHandler logEventHandler)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Listener name must be provided.", nameof(name));
        _logEventHandler = logEventHandler ?? throw new ArgumentNullException(nameof(logEventHandler));

    }

    /// <summary>
    /// Attach the listener to an already-started process.
    /// </summary>
    public void Attach(Process process)
    {
        if (process is null)
            throw new ArgumentNullException(nameof(process));

        if (_pumpTask is not null)
            throw new InvalidOperationException("Listener already attached to a process.");

        if (!process.StartInfo.RedirectStandardOutput || !process.StartInfo.RedirectStandardError)
            throw new InvalidOperationException("Process must redirect both standard output and error for logging.");

        _process = process;
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += _logEventHandler.HandleOutputData;
        process.ErrorDataReceived += _logEventHandler.HandleErrorData;
        process.Exited += OnProcessExited;

        _cts = new CancellationTokenSource();

        try
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }
        catch (InvalidOperationException ex)
        {
            CleanupAfterFailedAttach();
            throw new InvalidOperationException("Failed to begin reading redirected process streams.", ex);
        }

        if (process.HasExited)
        {
            OnProcessExited(process, EventArgs.Empty);
        }
    }


    private void OnProcessExited(object? sender, EventArgs args)
    {
        if (_process is not { } proc)
            return;
    }

    private void CleanupAfterFailedAttach()
    {
        if (_process is { } proc)
        {
            proc.OutputDataReceived -= _logEventHandler.HandleOutputData;
            proc.ErrorDataReceived -= _logEventHandler.HandleErrorData;
            proc.Exited -= OnProcessExited;
        }

        _process = null;

        if (_cts is not null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        _pumpTask = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_cts is not null)
        {
            _cts.Cancel();
            try
            {
                if (_pumpTask is not null)
                {
                    await _pumpTask.ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // ignore cancellation during disposal
            }

            _cts.Dispose();
            _cts = null;
        }

        if (_process is { } proc)
        {
            proc.OutputDataReceived -= _logEventHandler.HandleOutputData;
            proc.ErrorDataReceived -= _logEventHandler.HandleErrorData;
            proc.Exited -= OnProcessExited;
            _process = null;
        }
    }
}

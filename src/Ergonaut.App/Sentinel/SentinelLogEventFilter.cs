using Ergonaut.Core.LogIngestion;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Ergonaut.App.Sentinel;

public sealed class SentinelLogEventFilter : ILogEventFilter
{
    private readonly ILogger<SentinelLogEventFilter> _logger;
    private readonly SentinelConfig _config;

    public SentinelLogEventFilter(IOptions<SentinelConfig> options, ILogger<SentinelLogEventFilter> logger)
    {
        _config = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool Accept(ILogEvent logEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Evaluating log event: {LogEvent}", logEvent);

        if (logEvent == null)
            throw new ArgumentNullException(nameof(logEvent));

        if (RejectLogLevel(logEvent))
        {
            return false;
        }

        return true;
    }

    private bool RejectLogLevel(ILogEvent logEvent)
    {
        return logEvent.Level < _config.MinimumLevel;
    }
}

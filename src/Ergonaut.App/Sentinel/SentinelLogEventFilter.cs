using Ergonaut.Core.LogIngestion;
using Ergonaut.App.Services.ProjectScoped;
using Ergonaut.App.Models;
using Ergonaut.App.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Ergonaut.App.Sentinel;

public sealed class SentinelLogEventFilter : ILogEventFilter
{
    private readonly ILogger<SentinelLogEventFilter> _logger;
    private readonly SentinelConfig _config;

    private readonly IProjectService _projectService;
    private readonly IWorkItemService _workItemService;

    public SentinelLogEventFilter(IOptions<SentinelConfig> options, ILogger<SentinelLogEventFilter> logger, IProjectService projectService, IWorkItemService workItemService)
    {
        _config = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _workItemService = workItemService ?? throw new ArgumentNullException(nameof(workItemService));
        _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
    }

    async public Task<bool> Accept(ILogEvent logEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Evaluating log event: {LogEvent}", logEvent);

        if (logEvent == null)
            throw new ArgumentNullException(nameof(logEvent));

        if (RejectLogLevel(logEvent))
        {
            return false;
        }

        // IReadOnlyList<WorkItemRecord> existingWorkItems = await GetExistingWorkItems(cancellationToken);

        // existingWorkItems.ToList().FindAll(w =>  w.Source == logEvent.Source);

        return true;
    }

    private bool RejectLogLevel(ILogEvent logEvent)
    {
        return logEvent.Level < _config.MinimumLevel;
    }

    private async Task<IReadOnlyList<WorkItemRecord>> GetExistingWorkItems(CancellationToken cancellationToken)
    {
        ProjectRecord? project = await _projectService.GetProjectByName(_config.ProjectName, cancellationToken);
        if (project == null)
        {
            _logger.LogWarning("Project '{ProjectName}' not found.", _config.ProjectName);
            return Array.Empty<WorkItemRecord>();
        }
        return await _workItemService.ListAsync(projectId: project.Id, cancellationToken);
    }
}

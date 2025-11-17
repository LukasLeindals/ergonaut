using Ergonaut.Core.LogIngestion;
using Ergonaut.App.Services.ProjectScoped;
using Ergonaut.App.Models;
using Ergonaut.App.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Ergonaut.Core.Models.WorkItem;

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
            return false;

        IReadOnlyList<WorkItemRecord> existingWorkItems = await GetExistingWorkItems(cancellationToken);

        if (RejectMessageTemplate(logEvent, existingWorkItems))
            return false;



        return true;
    }

    private bool RejectLogLevel(ILogEvent logEvent)
    {
        return logEvent.Level < _config.MinimumLevel;
    }

    private bool RejectMessageTemplate(ILogEvent logEvent, IReadOnlyList<WorkItemRecord> existingWorkItems)
    {

        var useTemplate = !string.IsNullOrWhiteSpace(logEvent.MessageTemplate);
        if (!useTemplate)
            _logger.LogInformation("Log event has no message template; using message instead.");

        var attributeName = useTemplate ? LogIngestionConstants.MessageTemplateKey : LogIngestionConstants.MessageKey;
        var attributeValue = useTemplate ? logEvent.MessageTemplate : logEvent.Message;

        var matchingTemplates = existingWorkItems
            .Where(w => w.Status != WorkItemStatus.Done &&
                        ExtractSourceData<string?>(w, attributeName) == attributeValue)
            .ToList();

        if (matchingTemplates.Count > 0)
        {
            _logger.LogInformation("Rejecting log event with message template '{MessageTemplate}' because {Count} matching work items already exist.", logEvent.MessageTemplate, matchingTemplates.Count);
            return true;
        }

        return false;
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

    private T? ExtractSourceData<T>(IWorkItem workItem, string attributeName)
    {
        if (workItem.SourceData is null ||
            !workItem.SourceData.TryGetValue(attributeName, out var element) ||
            element is null)
        {
            return default;
        }

        var json = element.Value;

        try
        {
            // cheap path for strings
            if (typeof(T) == typeof(string) && json.ValueKind == JsonValueKind.String)
                return (T?)(object?)json.GetString();

            return json.Deserialize<T>();
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize source data attribute '{AttributeName}' in work item ID {WorkItemId}.", attributeName, workItem.Id);
            return default;
        }
    }
}

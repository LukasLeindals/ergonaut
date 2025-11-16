using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Ergonaut.Core.LogIngestion;
using Ergonaut.Core.Models;
using Ergonaut.Core.Models.WorkItem;
using Ergonaut.Core.Utils;
using Ergonaut.App.Services;
using Ergonaut.App.Services.ProjectScoped;
using Ergonaut.App.Models;
namespace Ergonaut.App.Sentinel;

public class WorkItemCreator : IWorkItemCreator
{
    private readonly SentinelConfig _config;
    private readonly ILogger<WorkItemCreator> _logger;

    private readonly IProjectService _projectService;
    private readonly IWorkItemService _workItemService;

    public WorkItemCreator(ILogger<WorkItemCreator> logger, IOptions<SentinelConfig> config, IProjectService projectService, IWorkItemService workItemService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
        _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        _workItemService = workItemService ?? throw new ArgumentNullException(nameof(workItemService));
    }
    public async Task CreateWorkItem(ILogEvent logEvent, CancellationToken cancellationToken = default)
    {
        ProjectRecord? project = await _projectService.GetProjectByName(_config.ProjectName, cancellationToken);
        project = await HandleProjectRecord(project, cancellationToken);

        CreateWorkItemRequest workItemRequest = await Convert(logEvent, project, cancellationToken);
        WorkItemRecord? result = await _workItemService.CreateAsync(project.Id, workItemRequest, cancellationToken);

        _logger.LogInformation("Created work item '{WorkItemTitle}' with ID {WorkItemId} in project '{ProjectName}'.", result.Title, result.Id, project.Title);
    }

    public async Task<ProjectRecord> HandleProjectRecord(ProjectRecord? project, CancellationToken cancellationToken)
    {
        if (project is not null)
        {
            return project;
        }

        _logger.LogInformation("Project '{ProjectName}' not found. Creating new project.", _config.ProjectName);
        var createRequest = new CreateProjectRequest
        {
            Title = _config.ProjectName,
            SourceLabel = SourceLabel.Sentinel,
            Description = "Auto-created project for Sentinel log events."
        };

        ProjectRecord createdProject = await _projectService.CreateAsync(createRequest, cancellationToken);
        _logger.LogInformation("Created new project '{ProjectName}' with ID {ProjectId}.", createdProject.Title, createdProject.Id);
        return createdProject;
    }

    private async Task<CreateWorkItemRequest> Convert(ILogEvent logEvent, ProjectRecord project, CancellationToken cancellationToken)
    {
        // Conversion logic to create a work item from log event
        var existing = await _workItemService.ListAsync(projectId: project.Id, cancellationToken);

        var workItemRequest = new CreateWorkItemRequest
        {
            Title = CreateTitle(existing),
            Description = logEvent.Message,
            SourceLabel = SourceLabel.Sentinel,
            Status = WorkItemStatus.New,
            Priority = MapLogLevelToPriority(logEvent.Level),
            SourceData = ExtractSourceData(logEvent)
        };

        return workItemRequest;
    }

    private static string CreateTitle(IEnumerable<WorkItemRecord> existingWorkItems)
    {
        string titlePrefix = "Sentinel Alert #";
        WorkItemRecord? lastExisting = existingWorkItems.OrderBy(w => w.CreatedAt).ToList().FindLast(w => w.Title.StartsWith(titlePrefix));

        if (lastExisting == null)
        {
            return $"{titlePrefix}1";
        }

        if (!int.TryParse(lastExisting.Title.Replace(titlePrefix, ""), out int lastSuffix))
        {
            lastSuffix = existingWorkItems.Count(w => w.Title.StartsWith(titlePrefix)) - 1;
        }

        return $"{titlePrefix}{lastSuffix + 1}";
    }

    private static WorkItemPriority? MapLogLevelToPriority(LogLevel level)
    {
        return level switch
        {
            LogLevel.Critical => WorkItemPriority.High,
            LogLevel.Error => WorkItemPriority.High,
            LogLevel.Warning => WorkItemPriority.Medium,
            LogLevel.Information => WorkItemPriority.Low,
            LogLevel.Debug => WorkItemPriority.Low,
            LogLevel.Trace => WorkItemPriority.Low,
            _ => null,
        };
    }

    private static Dictionary<string, JsonElement?>? ExtractSourceData(ILogEvent logEvent)
    {
        Dictionary<string, JsonElement?> sourceData = logEvent.Metadata is null
            ? new Dictionary<string, JsonElement?>()
            : logEvent.Metadata.ToDictionary(
                entry => entry.Key,
                entry => entry.Value);

        sourceData[LogIngestionConstants.MessageTemplateKey] = JsonUtils.ConvertToJsonElement(logEvent.MessageTemplate);

        foreach (var kvp in logEvent.ResourceAttributes)
        {
            sourceData[kvp.Key] = JsonUtils.ConvertToJsonElement(kvp.Value);
        }

        foreach (var kvp in logEvent.ScopeAttributes)
        {
            sourceData[kvp.Key] = JsonUtils.ConvertToJsonElement(kvp.Value);
        }

        foreach (var kvp in logEvent.Attributes)
        {
            sourceData[kvp.Key] = JsonUtils.ConvertToJsonElement(kvp.Value);
        }


        return sourceData.Count == 0 ? null : sourceData;
    }
}

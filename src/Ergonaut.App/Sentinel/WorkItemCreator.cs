using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ergonaut.Core.LogIngestion;
using Ergonaut.Core.Models;
using Ergonaut.Core.Models.WorkItem;
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

        _logger.LogInformation($"Project '{_config.ProjectName}' not found. Creating new project.");
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

        int? titleCountSuffix;

        WorkItemRecord? lastExisting = existing.ToList().OrderBy(w => w.CreatedAt).ToList().FindLast(w => w.SourceLabel == SourceLabel.Sentinel);
        if (lastExisting != null)
        {
            titleCountSuffix = int.TryParse(lastExisting.Title.Split('#').Last(), out int result) ? result : (int?)null;
        }
        else
        {
            titleCountSuffix = existing.ToList().Count(p => p.SourceLabel == SourceLabel.Sentinel);
        }

        var workItemRequest = new CreateWorkItemRequest
        {
            Title = $"Sentinel Alert #{(titleCountSuffix ?? 0) + 1}",
            Description = logEvent.Message,
            SourceLabel = SourceLabel.Sentinel,
            Status = WorkItemStatus.New,
            Priority = MapLogLevelToPriority(logEvent.Level),
            // SourceData # TODO: Map relevant log event data
        };

        return workItemRequest;
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
}

using Ergonaut.Core.Models;
using Ergonaut.Core.Exceptions;
using Ergonaut.App.Models;
using Ergonaut.App.Services.ProjectScoped;
using Ergonaut.Tests.Mock.App;
using Ergonaut.Core.Models.Project;
using Ergonaut.Core.Models.WorkItem;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Ergonaut.App.Tests.Services;

public sealed class WorkItemServiceTests
{
    private readonly Guid _existingProjectId = Guid.NewGuid();
    private readonly MockProjectRepository _projects = new();
    private readonly MockWorkItemRepository _workItems = new();
    private readonly ILogger<WorkItemService> _logger;

    public WorkItemServiceTests()
    {
        _logger = NullLogger<WorkItemService>.Instance;
        _projects.Add(new Project("Existing") { Id = _existingProjectId });
    }

    [Fact(DisplayName = "Throws when listing an unknown project")]
    public async Task ListAsync_Throws_When_ProjectMissing()
    {
        IWorkItemService service = CreateService();

        await Assert.ThrowsAsync<ProjectNotFoundException>(() => service.ListAsync(Guid.NewGuid()));
    }

    [Fact(DisplayName = "Lists only work items that belong to the scoped project")]
    public async Task ListAsync_Filters_By_ProjectId()
    {
        var otherProjectId = Guid.NewGuid();

        _workItems.Seed(
            new WorkItem(projectId: _existingProjectId, title: "Keep me", sourceLabel: SourceLabel.Ergonaut),
            new WorkItem(projectId: otherProjectId, title: "Ignore me", sourceLabel: SourceLabel.Ergonaut));

        var result = await CreateService().ListAsync(_existingProjectId);

        Assert.Single(result);
        Assert.All(result, summary => Assert.Equal(_existingProjectId, summary.ProjectId));
    }

    [Fact(DisplayName = "Creates work items using the scoped project identifier")]
    public async Task CreateAsync_Persists_Task_For_Project()
    {
        var request = new CreateWorkItemRequest { Title = "From tests", Description = "demo" };

        var created = await CreateService().CreateAsync(_existingProjectId, request);

        Assert.Equal(_existingProjectId, created.ProjectId);
        Assert.Equal("From tests", created.Title);
        Assert.Contains(_workItems.Items, workItem => workItem.Id == created.Id && workItem.ProjectId == _existingProjectId);
    }

    [Fact(DisplayName = "Deletes work items when they exist in the scoped project")]
    public async Task DeleteAsync_Removes_Task_When_Found()
    {
        var workItem = new WorkItem(projectId: _existingProjectId, title: "Delete me", sourceLabel: SourceLabel.Ergonaut);
        _workItems.Seed(workItem);

        var result = await CreateService().DeleteAsync(_existingProjectId, workItem.Id);

        Assert.True(result.Success);
        Assert.DoesNotContain(_workItems.Items, item => item.Id == workItem.Id);
    }
    private IWorkItemService CreateService() => new WorkItemService(_workItems, _projects, _logger);
}

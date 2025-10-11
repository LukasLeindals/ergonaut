using Ergonaut.App.Models;
using Ergonaut.App.Services;
using Ergonaut.Tests.Mock.App;
using Ergonaut.Core.Models.Project;
using Ergonaut.Core.Models.WorkItem;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Ergonaut.App.Tests.Services;

public sealed class ProjectScopedWorkItemServiceTests
{
    private readonly Guid _existingProjectId = Guid.NewGuid();
    private readonly MockProjectRepository _projects = new();
    private readonly MockWorkItemRepository _workItems = new();
    private readonly ILogger<ProjectScopedWorkItemService> _logger;

    public ProjectScopedWorkItemServiceTests()
    {
        _logger = NullLogger<ProjectScopedWorkItemService>.Instance;
        _projects.Add(new Project("Existing") { Id = _existingProjectId });
    }

    [Fact(DisplayName = "Throws when binding to an unknown project")]
    public void UseProject_Throws_When_ProjectMissing()
    {
        IProjectScopedWorkItemService service = CreateService();

        var act = () => service.UseProject(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => act());
    }

    [Fact(DisplayName = "Lists only work items that belong to the scoped project")]
    public async Task ListAsync_Filters_By_ProjectId()
    {
        var otherProjectId = Guid.NewGuid();
        IProjectScopedWorkItemService service = CreateService().UseProject(_existingProjectId);

        _workItems.Seed(
            new WorkItem(projectId: _existingProjectId, title: "Keep me"),
            new WorkItem(projectId: otherProjectId, title: "Ignore me"));

        var result = await service.ListAsync();

        Assert.Single(result);
        Assert.All(result, summary => Assert.Equal(_existingProjectId, summary.ProjectId));
    }

    [Fact(DisplayName = "Creates work items using the scoped project identifier")]
    public async Task CreateAsync_Persists_Task_For_Project()
    {
        IProjectScopedWorkItemService service = CreateService().UseProject(_existingProjectId);
        var request = new CreateWorkItemRequest { Title = "From tests", Description = "demo" };

        var created = await service.CreateAsync(request);

        Assert.Equal(_existingProjectId, created.ProjectId);
        Assert.Equal("From tests", created.Title);
        Assert.Contains(_workItems.Items, workItem => workItem.Id == created.Id && workItem.ProjectId == _existingProjectId);
    }

    [Fact(DisplayName = "Deletes work items when they exist in the scoped project")]
    public async Task DeleteAsync_Removes_Task_When_Found()
    {
        IProjectScopedWorkItemService service = CreateService().UseProject(_existingProjectId);
        var workItem = new WorkItem(projectId: _existingProjectId, title: "Delete me");
        _workItems.Seed(workItem);

        var result = await service.DeleteAsync(workItem.Id);

        Assert.True(result.Success);
        Assert.DoesNotContain(_workItems.Items, item => item.Id == workItem.Id);
    }

    private IProjectScopedWorkItemService CreateService() => new ProjectScopedWorkItemService(_workItems, _projects, _logger);
}

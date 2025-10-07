using Ergonaut.App.Features.WorkItems;
using Ergonaut.Core.Models.Project;
using Ergonaut.Core.Models.WorkItem;
using Ergonaut.Core.Repositories;
using Xunit;

namespace Ergonaut.App.Tests;

public sealed class ProjectScopedWorkItemServiceTests
{
    private readonly Guid _existingProjectId = Guid.NewGuid();
    private readonly FakeProjectRepository _projects = new();
    private readonly FakeWorkItemRepository _workItems = new();

    public ProjectScopedWorkItemServiceTests()
    {
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

    private IProjectScopedWorkItemService CreateService() => new ProjectScopedWorkItemService(_workItems, _projects);

    private sealed class FakeProjectRepository : IProjectRepository
    {
        private readonly Dictionary<Guid, IProject> _projects = new();

        public void Add(IProject project) => _projects[project.Id] = project;

        public Task<IProject?> GetAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult(_projects.TryGetValue(id, out var project) ? project : null);

        public Task<IReadOnlyList<IProject>> ListAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<IProject>>(_projects.Values.ToList());

        public Task<IProject> AddAsync(IProject project, CancellationToken ct = default)
        {
            Add(project);
            return Task.FromResult(project);
        }

        public Task UpdateAsync(IProject project, CancellationToken ct = default)
        {
            Add(project);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            _projects.Remove(id);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeWorkItemRepository : IWorkItemRepository
    {
        private readonly List<IWorkItem> _workItems = new();

        public IReadOnlyList<IWorkItem> Items => _workItems;

        public void Seed(params IWorkItem[] workItems)
        {
            foreach (var workItem in workItems)
            {
                _workItems.RemoveAll(existing => existing.Id == workItem.Id);
                _workItems.Add(workItem);
            }
        }

        public Task<IWorkItem?> GetAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult(_workItems.FirstOrDefault(t => t.Id == id));

        public Task<IReadOnlyList<IWorkItem>> ListAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<IWorkItem>>(_workItems.ToList());

        public Task<IReadOnlyList<IWorkItem>> ListByProjectAsync(Guid projectId, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<IWorkItem>>(_workItems.Where(t => t.ProjectId == projectId).ToList());

        public Task<IWorkItem> AddAsync(IWorkItem workItem, CancellationToken ct = default)
        {
            Seed(workItem);
            return Task.FromResult(workItem);
        }

        public Task UpdateAsync(IWorkItem workItem, CancellationToken ct = default)
        {
            Seed(workItem);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            _workItems.RemoveAll(workItem => workItem.Id == id);
            return Task.CompletedTask;
        }
    }
}

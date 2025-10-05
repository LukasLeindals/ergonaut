using Ergonaut.App.Features.Tasks;
using Ergonaut.App.Models;
using Ergonaut.Core.Models.Project;
using Ergonaut.Core.Models.Task;
using Ergonaut.Infrastructure.Repositories;

namespace Ergonaut.App.Tests;

public sealed class LocalProjectScopedTaskServiceTests
{
    private readonly Guid _existingProjectId = Guid.NewGuid();
    private readonly FakeProjectRepository _projects = new();
    private readonly FakeTaskRepository _tasks = new();

    public LocalProjectScopedTaskServiceTests()
    {
        _projects.Add(new LocalProject("Existing") { Id = _existingProjectId });
    }

    [Fact(DisplayName = "Throws when binding to an unknown project")]
    public void UseProject_Throws_When_ProjectMissing()
    {
        IProjectScopedTaskService service = CreateService();

        var act = () => service.UseProject(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => act());
    }

    [Fact(DisplayName = "Lists only tasks that belong to the scoped project")]
    public async Task ListAsync_Filters_By_ProjectId()
    {
        var otherProjectId = Guid.NewGuid();
        IProjectScopedTaskService service = CreateService().UseProject(_existingProjectId);

        _tasks.Seed(
            new LocalTask(_existingProjectId, "Keep me"),
            new LocalTask(otherProjectId, "Ignore me"));

        var result = await service.ListAsync();

        Assert.Single(result);
        Assert.All(result, summary => Assert.Equal(_existingProjectId, summary.ProjectId));
    }

    [Fact(DisplayName = "Creates tasks using the scoped project identifier")]
    public async Task CreateAsync_Persists_Task_For_Project()
    {
        IProjectScopedTaskService service = CreateService().UseProject(_existingProjectId);
        var request = new CreateTaskRequest { Title = "From tests", Description = "demo" };

        var created = await service.CreateAsync(request);

        Assert.Equal(_existingProjectId, created.ProjectId);
        Assert.Equal("From tests", created.Title);
        Assert.Contains(_tasks.Items, task => task.Id == created.Id && task.ProjectId == _existingProjectId);
    }

    [Fact(DisplayName = "Deletes tasks when they exist in the scoped project")]
    public async Task DeleteAsync_Removes_Task_When_Found()
    {
        IProjectScopedTaskService service = CreateService().UseProject(_existingProjectId);
        var task = new LocalTask(_existingProjectId, "Delete me");
        _tasks.Seed(task);

        var result = await service.DeleteAsync(task.Id);

        Assert.True(result.Success);
        Assert.DoesNotContain(_tasks.Items, t => t.Id == task.Id);
    }

    private IProjectScopedTaskService CreateService() => new LocalProjectScopedTaskService(_tasks, _projects);

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

    private sealed class FakeTaskRepository : ITaskRepository
    {
        private readonly List<ITask> _tasks = new();

        public IReadOnlyList<ITask> Items => _tasks;

        public void Seed(params ITask[] tasks)
        {
            foreach (var task in tasks)
            {
                _tasks.RemoveAll(existing => existing.Id == task.Id);
                _tasks.Add(task);
            }
        }

        public Task<ITask?> GetAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult(_tasks.FirstOrDefault(t => t.Id == id));

        public Task<IReadOnlyList<ITask>> ListAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<ITask>>(_tasks.ToList());

        public Task<IReadOnlyList<ITask>> ListByProjectAsync(Guid projectId, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<ITask>>(_tasks.Where(t => t.ProjectId == projectId).ToList());

        public Task<ITask> AddAsync(ITask task, CancellationToken ct = default)
        {
            Seed(task);
            return Task.FromResult(task);
        }

        public Task UpdateAsync(ITask task, CancellationToken ct = default)
        {
            Seed(task);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            _tasks.RemoveAll(task => task.Id == id);
            return Task.CompletedTask;
        }
    }
}

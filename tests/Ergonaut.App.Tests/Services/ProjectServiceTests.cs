using System;
using System.Collections.Generic;
using System.Linq;
using Ergonaut.App.Models;
using Ergonaut.App.Services;
using Ergonaut.Core.Models.Project;
using Ergonaut.Tests.Mock.App;
using Xunit;

namespace Ergonaut.App.Tests.Services;

public sealed class ProjectServiceTests
{
    private readonly MockProjectRepository _projects = new();
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        _service = new ProjectService(_projects);
    }

    [Fact(DisplayName = "CreateAsync persists a project and returns the mapped record")]
    public async Task CreateAsync_Persists_And_Maps_Project()
    {
        var request = new CreateProjectRequest { Title = "Telemetry" };

        ProjectRecord created = await _service.CreateAsync(request);

        Assert.Equal("Telemetry", created.Title);
        Assert.NotEqual(Guid.Empty, created.Id);

        var stored = await _projects.ListAsync();
        Assert.Single(stored);
        Assert.Equal("Telemetry", stored[0].Title);
    }

    [Fact(DisplayName = "ListAsync returns projects sorted by creation date descending")]
    public async Task ListAsync_Returns_Projects_In_Descending_Order()
    {
        var older = new Project("Alpha") { CreatedAt = new DateTime(2023, 12, 1), Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") };
        var newer = new Project("Beta") { CreatedAt = new DateTime(2024, 1, 1), Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") };
        _projects.Add(older);
        _projects.Add(newer);

        IReadOnlyList<ProjectRecord> results = await _service.ListAsync();

        Assert.Equal(new[] { "Beta", "Alpha" }, results.Select(r => r.Title).ToArray());
    }

    [Fact(DisplayName = "DeleteAsync removes an existing project and returns a success response")]
    public async Task DeleteAsync_Removes_Project_When_Found()
    {
        var project = new Project("Cleanup") { Id = Guid.NewGuid() };
        _projects.Add(project);

        var response = await _service.DeleteAsync(project.Id);

        Assert.True(response.Success);
        var remaining = await _projects.ListAsync();
        Assert.DoesNotContain(remaining, p => p.Id == project.Id);
    }
}

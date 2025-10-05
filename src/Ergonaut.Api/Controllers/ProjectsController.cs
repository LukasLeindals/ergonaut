using Ergonaut.App.Models;
using Ergonaut.App.Features.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ergonaut.Api.Controllers;

[ApiController]
[Route("api/v1/projects")]
[Authorize(Policy = "ProjectsRead")]
public sealed class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService) => _projectService = projectService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectSummary>>> Get(CancellationToken ct)
        => Ok(await _projectService.ListAsync(ct));

    [HttpPost]
    [Authorize(Policy = "ProjectsWrite")]
    public async Task<ActionResult<ProjectSummary>> Post([FromBody] CreateProjectRequest request, CancellationToken ct)
    {
        var created = await _projectService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "ProjectsWrite")]
    public async Task<ActionResult<DeletionResult>> Delete(Guid id, CancellationToken ct)
    {
        DeletionResult result = await _projectService.DeleteAsync(id, ct);
        return result.Success ? NoContent() : NotFound(result.Message);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectSummary>> GetById(Guid id, CancellationToken ct)
    {
        var project = await _projectService.GetAsync(id, ct);
        return project is null ? NotFound() : Ok(project);
    }
}

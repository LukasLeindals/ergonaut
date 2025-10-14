using Ergonaut.App.Models;
using Ergonaut.App.Services;
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
    public async Task<ActionResult<IReadOnlyList<ProjectRecord>>> Get(CancellationToken ct)
        => Ok(await _projectService.ListAsync(ct));

    [HttpPost]
    [Authorize(Policy = "ProjectsWrite")]
    [ProducesResponseType(typeof(ProjectRecord), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<ProjectRecord>> Post([FromBody] CreateProjectRequest request, CancellationToken ct)
    {
        var created = await _projectService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "ProjectsWrite")]
    public async Task<ActionResult<DeletionResponse>> Delete(Guid id, CancellationToken ct)
    {
        DeletionResponse result = await _projectService.DeleteAsync(id, ct);
        return result.Success ? NoContent() : NotFound(result.Message);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectRecord>> GetById(Guid id, CancellationToken ct)
    {
        var project = await _projectService.GetAsync(id, ct);
        return project is null ? NotFound() : Ok(project);
    }
}

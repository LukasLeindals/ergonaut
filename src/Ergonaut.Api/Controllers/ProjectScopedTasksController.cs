using Ergonaut.App.Features.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ergonaut.Api.Controllers;

[ApiController]
[Route("api/v1/{projectId:guid}/tasks")]
[Authorize(Policy = "TasksRead")]
public sealed class ProjectScopedTasksController : ControllerBase
{
    private readonly IProjectScopedTaskService _taskService;

    public ProjectScopedTasksController(IProjectScopedTaskService taskService) => _taskService = taskService;


    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskSummary>>> Get([FromRoute] Guid projectId, CancellationToken ct)
        => Ok(await _taskService.UseProject(projectId).ListAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskSummary>> GetById([FromRoute] Guid projectId, [FromRoute] Guid id, CancellationToken ct)
    {
        var task = await _taskService.UseProject(projectId).GetAsync(id, ct);
        return task is null ? NotFound() : Ok(task);
    }


    [HttpPost]
    [Authorize(Policy = "TasksWrite")]
    public async Task<ActionResult<TaskSummary>> Post([FromRoute] Guid projectId, [FromBody] CreateTaskRequest request, CancellationToken ct)
    {
        try
        {
            var created = await _taskService.UseProject(projectId).CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { projectId, id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


}

using Ergonaut.App.Features.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ergonaut.Api.Controllers;

[ApiController]
[Route("api/v1/{projectId:guid}/tasks")]
[Authorize(Policy = "TasksRead")]
public sealed class ProjectTasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public ProjectTasksController(ITaskService taskService) => _taskService = taskService;


    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskSummary>>> GetByProjectId(Guid projectId, CancellationToken ct)
        => Ok(await _taskService.ListByProjectAsync(projectId, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskSummary>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var task = await _taskService.GetAsync(id, ct);
        return task is null ? NotFound() : Ok(task);
    }


    [HttpPost]
    [Authorize(Policy = "TasksWrite")]
    public async Task<ActionResult<TaskSummary>> Post([FromBody] CreateTaskRequest request, [FromRoute] Guid projectId, CancellationToken ct)
    {
        try
        {
            var created = await _taskService.CreateAsync(projectId, request, ct);
            return CreatedAtAction(nameof(GetById), new { projectId, id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


}

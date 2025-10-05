using Ergonaut.App.Models;
using Ergonaut.App.Features.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ergonaut.Api.Controllers;

[ApiController]
[Route("api/v1/tasks")]
[Authorize(Policy = "TasksRead")]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService) => _taskService = taskService;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskSummary>> GetById(Guid id, CancellationToken ct)
    {
        var task = await _taskService.GetAsync(id, ct);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "TasksWrite")]
    public async Task<ActionResult<DeletionResult>> Delete(Guid id, CancellationToken ct)
    {
        DeletionResult result = await _taskService.DeleteAsync(id, ct);
        return result.Success ? NoContent() : NotFound(result.Message);
    }


}



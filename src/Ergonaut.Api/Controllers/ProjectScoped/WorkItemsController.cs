using Ergonaut.App.Models;
using Ergonaut.App.Services.ProjectScoped;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ergonaut.Api.Controllers.ProjectScoped;

[ApiController]
[Route("api/v1/{projectId:guid}/work-items")]
[Authorize(Policy = "WorkItemsRead")]
public sealed class WorkItemsController : ControllerBase
{
    private readonly IWorkItemService _workItemService;

    public WorkItemsController(IWorkItemService workItemService) => _workItemService = workItemService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WorkItemRecord>>> Get([FromRoute] Guid projectId, CancellationToken ct)
    {
        var workItems = await _workItemService.ListAsync(projectId, ct);
        return Ok(workItems);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkItemRecord>> GetById([FromRoute] Guid projectId, [FromRoute] Guid id, CancellationToken ct)
    {
        var workItem = await _workItemService.GetAsync(projectId, id, ct);
        return workItem is null ? NotFound() : Ok(workItem);
    }


    [HttpPost]
    [Authorize(Policy = "WorkItemsWrite")]
    public async Task<ActionResult<WorkItemRecord>> Post([FromRoute] Guid projectId, [FromBody] CreateWorkItemRequest request, CancellationToken ct)
    {
        var created = await _workItemService.CreateAsync(projectId, request, ct);
        if (created is null)
            return BadRequest("Failed to create work item.");
        return CreatedAtAction(nameof(GetById), new { projectId, id = created.Id }, created);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "WorkItemsWrite")]
    public async Task<ActionResult<DeletionResponse>> Delete([FromRoute] Guid projectId, [FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _workItemService.DeleteAsync(projectId, id, ct);
        return result.Success ? NoContent() : NotFound(result.Message);
    }
}

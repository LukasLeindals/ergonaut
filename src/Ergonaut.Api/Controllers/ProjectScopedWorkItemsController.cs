using Ergonaut.App.Features.WorkItems;
using Ergonaut.App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ergonaut.Api.Controllers;

[ApiController]
[Route("api/v1/{projectId:guid}/work-items")]
[Authorize(Policy = "WorkItemsRead")]
public sealed class ProjectScopedWorkItemsController : ControllerBase
{
    private readonly IProjectScopedWorkItemService _workItemService;

    public ProjectScopedWorkItemsController(IProjectScopedWorkItemService workItemService) => _workItemService = workItemService;


    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WorkItemSummary>>> Get([FromRoute] Guid projectId, CancellationToken ct)
        => Ok(await _workItemService.UseProject(projectId).ListAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkItemSummary>> GetById([FromRoute] Guid projectId, [FromRoute] Guid id, CancellationToken ct)
    {
        var workItem = await _workItemService.UseProject(projectId).GetAsync(id, ct);
        return workItem is null ? NotFound() : Ok(workItem);
    }


    [HttpPost]
    [Authorize(Policy = "WorkItemsWrite")]
    public async Task<ActionResult<WorkItemSummary>> Post([FromRoute] Guid projectId, [FromBody] CreateWorkItemRequest request, CancellationToken ct)
    {
        try
        {
            var created = await _workItemService.UseProject(projectId).CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { projectId, id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "WorkItemsWrite")]
    public async Task<ActionResult<DeletionResult>> Delete([FromRoute] Guid projectId, [FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _workItemService.UseProject(projectId).DeleteAsync(id, ct);
        return result.Success ? NoContent() : NotFound(result.Message);
    }


}

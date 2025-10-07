using System;
using System.Threading;
using System.Threading.Tasks;
using Ergonaut.App.Models;
using Ergonaut.App.Features.WorkItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ergonaut.Api.Controllers;

[ApiController]
[Route("api/v1/work-items")]
[Authorize(Policy = "WorkItemsRead")]
public sealed class WorkItemsController : ControllerBase
{
    private readonly IWorkItemService _workItemService;

    public WorkItemsController(IWorkItemService workItemService) => _workItemService = workItemService;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkItemSummary>> GetById(Guid id, CancellationToken ct)
    {
        var workItem = await _workItemService.GetAsync(id, ct);
        return workItem is null ? NotFound() : Ok(workItem);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "WorkItemsWrite")]
    public async Task<ActionResult<DeletionResult>> Delete(Guid id, CancellationToken ct)
    {
        DeletionResult result = await _workItemService.DeleteAsync(id, ct);
        return result.Success ? NoContent() : NotFound(result.Message);
    }


}

using ITask = Ergonaut.Core.Models.Task.ITask;
using System.ComponentModel.DataAnnotations;

namespace Ergonaut.App.Features.Tasks;

public sealed record TaskSummary(Guid ProjectId, Guid Id, string Title, string? Description)
{
    public static TaskSummary FromTask(ITask task) =>
        new(task.ProjectId, task.Id, task.Title, task.Description ?? string.Empty);
}

public sealed class CreateTaskRequest
{

    [Required(ErrorMessage = "Please give your task a title.")]
    [StringLength(200, ErrorMessage = "Keep the name under 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Keep the description under 1000 characters.")]
    public string? Description { get; set; }
}




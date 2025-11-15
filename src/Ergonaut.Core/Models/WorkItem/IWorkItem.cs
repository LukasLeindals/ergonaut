using System.Text.Json;
namespace Ergonaut.Core.Models.WorkItem
{
    /// <summary>
    /// Represents a work item in the Ergonaut planning system.
    /// </summary>
    public interface IWorkItem
    {
        /// <summary>
        /// The unique identifier for the work item.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The identifier of the project this work item belongs to.
        /// </summary>
        Guid ProjectId { get; }

        /// <summary>
        /// The title of the work item.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The description of the work item.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// The date and time when the work item was created.
        /// </summary>
        DateTime CreatedAt { get; }

        /// <summary>
        /// The date and time when the work item was last updated.
        /// </summary>
        DateTime UpdatedAt { get; }

        /// <summary>
        /// The current status of the work item.
        /// </summary>
        WorkItemStatus Status { get; }

        /// <summary>
        /// The priority level of the work item.
        /// </summary>
        WorkItemPriority? Priority { get; }

        /// <summary>
        /// The due date for the work item.
        /// </summary>
        DateTime? DueDate { get; }

        /// <summary>
        /// The source of the work item (e.g., local).
        /// </summary>
        SourceLabel? SourceLabel { get; }

        /// <summary>
        /// Data specific to the source. This should only be modified by the source handler.
        /// </summary>
        Dictionary<string, JsonElement?>? SourceData { get; }

        /// <summary>
        /// Updates the work item with the provided details.
        /// </summary>
        /// <param name="title">The new title of the work item.</param>
        /// <param name="description">The new description of the work item.</param>
        /// <param name="status">The new status of the work item.</param>
        /// <param name="priority">The new priority level of the work item.</param>
        /// <param name="dueDate">The new due date of the work item.</param>
        /// <param name="sourceLabel">The new source label of the work item.</param>
        /// <param name="sourceData">The new source data of the work item.</param>
        IWorkItem Update(string title, WorkItemStatus status, string? description = null, WorkItemPriority? priority = null, DateTime? dueDate = null, SourceLabel? sourceLabel = null, Dictionary<string, JsonElement?>? sourceData = null);

    }
}

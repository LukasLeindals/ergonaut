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
        /// The source of the work item (e.g., local).
        /// </summary>
        WorkItemSourceLabel Source { get; }

        /// <summary>
        /// Data specific to the source. This should only be modified by the source handler.
        /// </summary>
        Dictionary<string, JsonElement?>? SourceData { get; }

    }
}

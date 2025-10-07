namespace Ergonaut.Core.Models.WorkItem
{
    /// <summary>
    /// Represents a work item in the Ergonaut planning system.
    /// </summary>
    public partial class LocalWorkItem : IWorkItem
    {

        public Guid Id { get; init; } = Guid.NewGuid();

        public Guid ProjectId { get; private set; }

        public string Title { get; private set; }

        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the priority level of the work item.
        /// </summary>
        public LocalWorkItemPriority? Priority { get; private set; } = null;

        /// <summary>
        /// Gets or sets the current status of the work item.
        /// </summary>
        public LocalWorkItemStatus Status { get; private set; } = LocalWorkItemStatus.New;

        /// <summary>
        /// Gets or sets the date and time when the work item was created.
        /// </summary>
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date and time when the work item was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the due date for the work item.
        /// </summary>
        public DateTime? DueDate { get; private set; } = null;

        /// <summary>
        /// Base constructor for the work item class.
        /// </summary>
        /// <param name="projectId">The identifier of the project this work item belongs to.</param>
        /// <param name="title">The title of the work item.</param>
        public LocalWorkItem(Guid projectId, string title, string? description = null)
        {
            ProjectId = projectId;
            Title = NormalizeTitle(title);
            Description = description ?? string.Empty;
        }
    }
}

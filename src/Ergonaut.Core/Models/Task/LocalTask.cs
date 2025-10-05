namespace Ergonaut.Core.Models.Task
{
    /// <summary>
    /// Represents a task item in the Ergonaut task management system.
    /// </summary>
    public partial class LocalTask : ITask
    {

        public Guid Id { get; init; } = Guid.NewGuid();

        public Guid ProjectId { get; private set; }

        public string Title { get; private set; }

        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the priority level of the task.
        /// </summary>
        public LocalTaskPriority? Priority { get; private set; } = null;

        /// <summary>
        /// Gets or sets the current status of the task.
        /// </summary>
        public LocalTaskStatus Status { get; private set; } = LocalTaskStatus.New;

        /// <summary>
        /// Gets or sets the date and time when the task was created.
        /// </summary>
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date and time when the task was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the due date for the task.
        /// </summary>
        public DateTime? DueDate { get; private set; } = null;

        /// <summary>
        /// Base constructor for the Task class.
        /// </summary>
        /// <param name="projectId">The identifier of the project this task belongs to.</param>
        /// <param name="title">The title of the task.</param>
        public LocalTask(Guid projectId, string title, string? description = null)
        {
            ProjectId = projectId;
            Title = NormalizeTitle(title);
            Description = description ?? string.Empty;
        }
    }
}

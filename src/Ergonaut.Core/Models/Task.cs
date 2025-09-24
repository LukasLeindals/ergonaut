namespace Ergonaut.Core.Models
{
    /// <summary>
    /// Represents a task item in the Ergonaut task management system.
    /// </summary>
    public partial class Task
    {
        /// <summary>
        /// Gets or sets the unique identifier for the task.
        /// </summary>
        public Guid Id { get; init; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the identifier of the project this task belongs to.
        /// </summary>
        public Guid ProjectId { get; private set; }

        /// <summary>
        /// Gets or sets the title of the task.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets or sets the description of the task.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the priority level of the task.
        /// </summary>
        public TaskPriority? Priority { get; private set; } = null;

        /// <summary>
        /// Gets or sets the current status of the task.
        /// </summary>
        public TaskStatus Status { get; private set; } = TaskStatus.New;

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
        public Task(Guid projectId, string title)
        {
            ProjectId = projectId;
            Title = NormalizeTitle(title);
        }
    }
}

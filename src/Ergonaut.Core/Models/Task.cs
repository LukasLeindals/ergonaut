
namespace Ergonaut.Core.Models
{
    /// <summary>
    /// Represents a task item in the Ergonaut task management system.
    /// </summary>
    public class Task
    {
        /// <summary>
        /// Gets or sets the unique identifier for the task.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the task.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the task.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the priority level of the task.
        /// </summary>
        public TaskPriority Priority { get; set; }

        /// <summary>
        /// Gets or sets the current status of the task.
        /// </summary>
        public TaskStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the task was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the task was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the due date for the task.
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the Task class.
        /// </summary>
        public Task()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Status = TaskStatus.Created;
            Priority = TaskPriority.Medium;
        }
    }

    /// <summary>
    /// Defines the possible statuses for tasks.
    /// </summary>
    public enum TaskStatus
    {
        Created = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3,
        OnHold = 4
    }
}

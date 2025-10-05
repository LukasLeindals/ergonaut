namespace Ergonaut.Core.Models.Task
{
    /// <summary>
    /// Represents a task item in the Ergonaut task management system.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// The unique identifier for the task.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The identifier of the project this task belongs to.
        /// </summary>
        Guid ProjectId { get; }

        /// <summary>
        /// The title of the task.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The description of the task.
        /// </summary>
        string Description { get; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Ergonaut.Core.Models;
using Ergonaut.Core.Utilities;

namespace Ergonaut.Sentinel.Repositories
{
    /// <summary>
    /// Repository for managing TaskItem entities.
    /// </summary>
    public class TaskRepository
    {
        private readonly List<TaskItem> _tasks;

        /// <summary>
        /// Initializes a new instance of the TaskRepository class.
        /// </summary>
        public TaskRepository()
        {
            _tasks = new List<TaskItem>();
            Logger.LogInfo("TaskRepository initialized");
        }

        /// <summary>
        /// Gets all tasks.
        /// </summary>
        /// <returns>A collection of all tasks.</returns>
        public IEnumerable<TaskItem> GetAll()
        {
            Logger.LogDebug($"Retrieved {_tasks.Count} tasks");
            return _tasks.AsReadOnly();
        }

        /// <summary>
        /// Gets a task by its identifier.
        /// </summary>
        /// <param name="id">The task identifier.</param>
        /// <returns>The task if found; otherwise, null.</returns>
        public TaskItem? GetById(Guid id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            Logger.LogDebug($"Retrieved task with ID: {id}, Found: {task != null}");
            return task;
        }

        /// <summary>
        /// Gets tasks by their status.
        /// </summary>
        /// <param name="status">The task status to filter by.</param>
        /// <returns>A collection of tasks with the specified status.</returns>
        public IEnumerable<TaskItem> GetByStatus(Core.Models.TaskStatus status)
        {
            var tasks = _tasks.Where(t => t.Status == status).ToList();
            Logger.LogDebug($"Retrieved {tasks.Count} tasks with status: {status}");
            return tasks;
        }

        /// <summary>
        /// Adds a new task.
        /// </summary>
        /// <param name="task">The task to add.</param>
        /// <returns>The added task.</returns>
        public TaskItem Add(TaskItem task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            _tasks.Add(task);
            Logger.LogInfo($"Added new task: {task.Title} (ID: {task.Id})");
            return task;
        }

        /// <summary>
        /// Updates an existing task.
        /// </summary>
        /// <param name="task">The task to update.</param>
        /// <returns>The updated task if found; otherwise, null.</returns>
        public TaskItem? Update(TaskItem task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var existingTask = _tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existingTask != null)
            {
                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.Priority = task.Priority;
                existingTask.Status = task.Status;
                existingTask.DueDate = task.DueDate;
                existingTask.UpdatedAt = DateTime.UtcNow;

                Logger.LogInfo($"Updated task: {task.Title} (ID: {task.Id})");
                return existingTask;
            }

            Logger.LogWarning($"Task not found for update: ID {task.Id}");
            return null;
        }

        /// <summary>
        /// Deletes a task by its identifier.
        /// </summary>
        /// <param name="id">The task identifier.</param>
        /// <returns>True if the task was deleted; otherwise, false.</returns>
        public bool Delete(Guid id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                _tasks.Remove(task);
                Logger.LogInfo($"Deleted task: {task.Title} (ID: {id})");
                return true;
            }

            Logger.LogWarning($"Task not found for deletion: ID {id}");
            return false;
        }
    }
}
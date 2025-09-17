using System;
using System.Collections.Generic;
using Ergonaut.Core.Models;
using Ergonaut.Core.Utilities;
using Ergonaut.Sentinel.Repositories;
using Ergonaut.Sentinel.Services;

namespace Ergonaut.Sentinel.Controllers
{
    /// <summary>
    /// Controller for managing task operations and orchestrating task-related services.
    /// </summary>
    public class TaskController
    {
        private readonly TaskRepository _taskRepository;
        private readonly TaskGenerator _taskGenerator;
        private readonly LogProcessor _logProcessor;

        /// <summary>
        /// Initializes a new instance of the TaskController class.
        /// </summary>
        public TaskController()
        {
            _taskRepository = new TaskRepository();
            _taskGenerator = new TaskGenerator();
            _logProcessor = new LogProcessor();
            Logger.LogInfo("TaskController initialized");
        }

        /// <summary>
        /// Initializes a new instance of the TaskController class with custom dependencies.
        /// </summary>
        /// <param name="taskRepository">The task repository.</param>
        /// <param name="taskGenerator">The task generator.</param>
        /// <param name="logProcessor">The log processor.</param>
        public TaskController(TaskRepository taskRepository, TaskGenerator taskGenerator, LogProcessor logProcessor)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _taskGenerator = taskGenerator ?? throw new ArgumentNullException(nameof(taskGenerator));
            _logProcessor = logProcessor ?? throw new ArgumentNullException(nameof(logProcessor));
            Logger.LogInfo("TaskController initialized with custom dependencies");
        }

        /// <summary>
        /// Gets all tasks.
        /// </summary>
        /// <returns>A collection of all tasks.</returns>
        public IEnumerable<TaskItem> GetAllTasks()
        {
            Logger.LogDebug("Retrieving all tasks");
            return _taskRepository.GetAll();
        }

        /// <summary>
        /// Gets a task by its identifier.
        /// </summary>
        /// <param name="id">The task identifier.</param>
        /// <returns>The task if found; otherwise, null.</returns>
        public TaskItem? GetTaskById(Guid id)
        {
            Logger.LogDebug($"Retrieving task by ID: {id}");
            return _taskRepository.GetById(id);
        }

        /// <summary>
        /// Gets tasks by status.
        /// </summary>
        /// <param name="status">The task status.</param>
        /// <returns>A collection of tasks with the specified status.</returns>
        public IEnumerable<TaskItem> GetTasksByStatus(Core.Models.TaskStatus status)
        {
            Logger.LogDebug($"Retrieving tasks by status: {status}");
            return _taskRepository.GetByStatus(status);
        }

        /// <summary>
        /// Creates a new task.
        /// </summary>
        /// <param name="title">The task title.</param>
        /// <param name="description">The task description.</param>
        /// <param name="priority">The task priority.</param>
        /// <param name="dueDate">The task due date.</param>
        /// <returns>The created task.</returns>
        public TaskItem CreateTask(string title, string description, TaskPriority priority = TaskPriority.Medium, DateTime? dueDate = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title cannot be null or empty", nameof(title));
            }

            var task = new TaskItem
            {
                Title = title,
                Description = description ?? string.Empty,
                Priority = priority,
                DueDate = dueDate
            };

            var createdTask = _taskRepository.Add(task);
            Logger.LogInfo($"Created new task: {createdTask.Title}");
            return createdTask;
        }

        /// <summary>
        /// Updates an existing task.
        /// </summary>
        /// <param name="id">The task identifier.</param>
        /// <param name="title">The new task title.</param>
        /// <param name="description">The new task description.</param>
        /// <param name="priority">The new task priority.</param>
        /// <param name="status">The new task status.</param>
        /// <param name="dueDate">The new task due date.</param>
        /// <returns>The updated task if found; otherwise, null.</returns>
        public TaskItem? UpdateTask(Guid id, string? title = null, string? description = null, 
            TaskPriority? priority = null, Core.Models.TaskStatus? status = null, DateTime? dueDate = null)
        {
            var existingTask = _taskRepository.GetById(id);
            if (existingTask == null)
            {
                Logger.LogWarning($"Task not found for update: {id}");
                return null;
            }

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(title))
                existingTask.Title = title;
            if (description != null)
                existingTask.Description = description;
            if (priority.HasValue)
                existingTask.Priority = priority.Value;
            if (status.HasValue)
                existingTask.Status = status.Value;
            if (dueDate.HasValue)
                existingTask.DueDate = dueDate;

            return _taskRepository.Update(existingTask);
        }

        /// <summary>
        /// Deletes a task.
        /// </summary>
        /// <param name="id">The task identifier.</param>
        /// <returns>True if the task was deleted; otherwise, false.</returns>
        public bool DeleteTask(Guid id)
        {
            Logger.LogDebug($"Deleting task: {id}");
            return _taskRepository.Delete(id);
        }

        /// <summary>
        /// Generates tasks based on context.
        /// </summary>
        /// <param name="context">The context for task generation.</param>
        /// <returns>The generated task.</returns>
        public TaskItem GenerateTask(string context)
        {
            var generatedTask = _taskGenerator.GenerateTask(context);
            var savedTask = _taskRepository.Add(generatedTask);
            Logger.LogInfo($"Generated and saved task: {savedTask.Title}");
            return savedTask;
        }

        /// <summary>
        /// Generates multiple tasks based on contexts.
        /// </summary>
        /// <param name="contexts">The contexts for task generation.</param>
        /// <returns>A collection of generated tasks.</returns>
        public IEnumerable<TaskItem> GenerateTasks(IEnumerable<string> contexts)
        {
            var generatedTasks = _taskGenerator.GenerateTasks(contexts);
            var savedTasks = new List<TaskItem>();

            foreach (var task in generatedTasks)
            {
                savedTasks.Add(_taskRepository.Add(task));
            }

            Logger.LogInfo($"Generated and saved {savedTasks.Count} tasks");
            return savedTasks;
        }

        /// <summary>
        /// Processes log content and creates tasks from extracted patterns.
        /// </summary>
        /// <param name="logContent">The log content to process.</param>
        /// <returns>A collection of tasks created from log processing.</returns>
        public IEnumerable<TaskItem> ProcessLogsAndCreateTasks(string logContent)
        {
            var extractedTasks = _logProcessor.ProcessLogEntry(logContent);
            var savedTasks = new List<TaskItem>();

            foreach (var task in extractedTasks)
            {
                savedTasks.Add(_taskRepository.Add(task));
            }

            Logger.LogInfo($"Processed logs and created {savedTasks.Count} tasks");
            return savedTasks;
        }

        /// <summary>
        /// Analyzes log patterns and creates tasks for recurring issues.
        /// </summary>
        /// <returns>A collection of tasks for addressing recurring patterns.</returns>
        public IEnumerable<TaskItem> AnalyzeLogsAndCreateTasks()
        {
            var patternTasks = _logProcessor.AnalyzeLogPatterns();
            var savedTasks = new List<TaskItem>();

            foreach (var task in patternTasks)
            {
                savedTasks.Add(_taskRepository.Add(task));
            }

            Logger.LogInfo($"Analyzed log patterns and created {savedTasks.Count} tasks");
            return savedTasks;
        }

        /// <summary>
        /// Gets summary information about the task management system.
        /// </summary>
        /// <returns>Task management summary.</returns>
        public TaskManagementSummary GetSummary()
        {
            var allTasks = _taskRepository.GetAll();
            var stats = _logProcessor.GetProcessingStats();

            return new TaskManagementSummary
            {
                TotalTasks = allTasks.Count(),
                CompletedTasks = _taskRepository.GetByStatus(Core.Models.TaskStatus.Completed).Count(),
                InProgressTasks = _taskRepository.GetByStatus(Core.Models.TaskStatus.InProgress).Count(),
                PendingTasks = _taskRepository.GetByStatus(Core.Models.TaskStatus.Created).Count(),
                LogsProcessed = stats.TotalLogsProcessed
            };
        }
    }

    /// <summary>
    /// Provides summary information about task management.
    /// </summary>
    public class TaskManagementSummary
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int PendingTasks { get; set; }
        public int LogsProcessed { get; set; }
    }
}
using System;
using System.Collections.Generic;
using Ergonaut.Core.Models;
using Ergonaut.Core.Utilities;

namespace Ergonaut.Sentinel.Services
{
    /// <summary>
    /// Service responsible for generating intelligent tasks based on various inputs and patterns.
    /// </summary>
    public class TaskGenerator
    {
        private readonly Random _random;

        /// <summary>
        /// Initializes a new instance of the TaskGenerator class.
        /// </summary>
        public TaskGenerator()
        {
            _random = new Random();
            Logger.LogInfo("TaskGenerator initialized");
        }

        /// <summary>
        /// Generates a single task based on the provided context.
        /// </summary>
        /// <param name="context">The context for task generation.</param>
        /// <returns>A generated task item.</returns>
        public TaskItem GenerateTask(string context)
        {
            if (string.IsNullOrWhiteSpace(context))
            {
                throw new ArgumentException("Context cannot be null or empty", nameof(context));
            }

            var task = new TaskItem
            {
                Title = GenerateTaskTitle(context),
                Description = GenerateTaskDescription(context),
                Priority = GenerateTaskPriority(),
                DueDate = GenerateTaskDueDate()
            };

            Logger.LogInfo($"Generated task: {task.Title}");
            return task;
        }

        /// <summary>
        /// Generates multiple tasks based on the provided contexts.
        /// </summary>
        /// <param name="contexts">The contexts for task generation.</param>
        /// <returns>A collection of generated task items.</returns>
        public IEnumerable<TaskItem> GenerateTasks(IEnumerable<string> contexts)
        {
            if (contexts == null)
            {
                throw new ArgumentNullException(nameof(contexts));
            }

            var tasks = new List<TaskItem>();
            foreach (var context in contexts)
            {
                if (!string.IsNullOrWhiteSpace(context))
                {
                    tasks.Add(GenerateTask(context));
                }
            }

            Logger.LogInfo($"Generated {tasks.Count} tasks from provided contexts");
            return tasks;
        }

        /// <summary>
        /// Generates tasks based on recurring patterns.
        /// </summary>
        /// <param name="pattern">The recurring pattern type.</param>
        /// <param name="count">The number of tasks to generate.</param>
        /// <returns>A collection of generated recurring tasks.</returns>
        public IEnumerable<TaskItem> GenerateRecurringTasks(RecurrencePattern pattern, int count)
        {
            if (count <= 0)
            {
                throw new ArgumentException("Count must be greater than zero", nameof(count));
            }

            var tasks = new List<TaskItem>();
            for (int i = 0; i < count; i++)
            {
                var task = new TaskItem
                {
                    Title = GenerateRecurringTitle(pattern, i + 1),
                    Description = GenerateRecurringDescription(pattern),
                    Priority = TaskPriority.Medium,
                    DueDate = CalculateRecurringDueDate(pattern, i)
                };
                tasks.Add(task);
            }

            Logger.LogInfo($"Generated {tasks.Count} recurring tasks with pattern: {pattern}");
            return tasks;
        }

        private string GenerateTaskTitle(string context)
        {
            var titleTemplates = new[]
            {
                $"Process {context}",
                $"Review {context}",
                $"Analyze {context}",
                $"Optimize {context}",
                $"Complete {context}"
            };

            return titleTemplates[_random.Next(titleTemplates.Length)];
        }

        private string GenerateTaskDescription(string context)
        {
            return $"Intelligently generated task for: {context}. This task requires attention and processing.";
        }

        private TaskPriority GenerateTaskPriority()
        {
            var priorities = Enum.GetValues<TaskPriority>();
            return priorities[_random.Next(priorities.Length)];
        }

        private DateTime? GenerateTaskDueDate()
        {
            // Generate due date within next 30 days
            var daysToAdd = _random.Next(1, 31);
            return DateTime.UtcNow.AddDays(daysToAdd);
        }

        private string GenerateRecurringTitle(RecurrencePattern pattern, int sequence)
        {
            return pattern switch
            {
                RecurrencePattern.Daily => $"Daily Task #{sequence}",
                RecurrencePattern.Weekly => $"Weekly Task #{sequence}",
                RecurrencePattern.Monthly => $"Monthly Task #{sequence}",
                _ => $"Recurring Task #{sequence}"
            };
        }

        private string GenerateRecurringDescription(RecurrencePattern pattern)
        {
            return pattern switch
            {
                RecurrencePattern.Daily => "Daily recurring task generated automatically",
                RecurrencePattern.Weekly => "Weekly recurring task generated automatically",
                RecurrencePattern.Monthly => "Monthly recurring task generated automatically",
                _ => "Recurring task generated automatically"
            };
        }

        private DateTime CalculateRecurringDueDate(RecurrencePattern pattern, int sequence)
        {
            return pattern switch
            {
                RecurrencePattern.Daily => DateTime.UtcNow.AddDays(sequence + 1),
                RecurrencePattern.Weekly => DateTime.UtcNow.AddDays((sequence + 1) * 7),
                RecurrencePattern.Monthly => DateTime.UtcNow.AddMonths(sequence + 1),
                _ => DateTime.UtcNow.AddDays(sequence + 1)
            };
        }
    }

    /// <summary>
    /// Defines the types of recurring patterns for task generation.
    /// </summary>
    public enum RecurrencePattern
    {
        Daily,
        Weekly,
        Monthly
    }
}
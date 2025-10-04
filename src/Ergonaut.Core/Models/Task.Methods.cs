using System;

namespace Ergonaut.Core.Models
{
    public partial class Task
    {
        public void UpdateTitle(string title)
        {
            Title = NormalizeTitle(title);
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDescription(string? description)
        {
            Description = NormalizeDescription(description);
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePriority(TaskPriority? priority)
        {
            Priority = priority;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(TaskStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDueDate(DateTime? dueDate)
        {
            DueDate = dueDate;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateProjectId(Guid projectId)
        {
            ProjectId = projectId;
            UpdatedAt = DateTime.UtcNow;
        }



        private static string NormalizeTitle(string title)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);
            return title.Trim();
        }

        private static string NormalizeDescription(string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return string.Empty;
            }

            return description.Trim();
        }
    }
}

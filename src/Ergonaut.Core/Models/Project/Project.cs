namespace Ergonaut.Core.Models.Project
{
    public class Project : IProject
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public string Title { get; private set; }

        public string? Description { get; private set; } = null;

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        public SourceLabel? SourceLabel { get; private set; }

        public Project(string title)
            : this(title, null, null)
        {
        }

        public Project(string title, string? description, SourceLabel? sourceLabel)
        {
            Title = NormalizeTitle(title);
            Description = description;
            SourceLabel = sourceLabel;
        }

        public void Rename(string title)
        {
            Title = NormalizeTitle(title);
            UpdatedAt = DateTime.UtcNow;
        }

        private static string NormalizeTitle(string title)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);
            return title.Trim();
        }

        public IProject Update(string title, string? description, SourceLabel? sourceLabel)
        {
            Title = NormalizeTitle(title);
            Description = description;
            SourceLabel = sourceLabel;
            
            UpdatedAt = DateTime.UtcNow;
            return this;
        }
    }
}

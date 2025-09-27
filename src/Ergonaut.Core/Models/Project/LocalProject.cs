namespace Ergonaut.Core.Models.Project
{
    public class LocalProject : IProject
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public string Title { get; private set; }

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        public ProjectSource Source { get; init; } = ProjectSource.Local;

        public LocalProject(string title)
        {
            Title = NormalizeTitle(title);
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
    }
}

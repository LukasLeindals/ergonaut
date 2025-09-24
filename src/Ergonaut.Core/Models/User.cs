using System;

namespace Ergonaut.Core.Models
{
    public sealed class User
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public string Username { get; private set; }

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        public User(string username)
        {
            Username = NormalizeUsername(username);
        }

        public void Rename(string username)
        {
            Username = NormalizeUsername(username);
            UpdatedAt = DateTime.UtcNow;
        }

        private static string NormalizeUsername(string username)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(username);
            return username.Trim();
        }
    }
}

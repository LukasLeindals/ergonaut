using Ergonaut.Core.Models.Project;

namespace Ergonaut.App.Features.Projects;

public sealed class LocalProjectFactory : IProjectFactory
{
    public IProject Create(string title) => new LocalProject(title);
}

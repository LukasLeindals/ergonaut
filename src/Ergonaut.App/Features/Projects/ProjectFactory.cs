using Ergonaut.Core.Models.Project;

namespace Ergonaut.App.Features.Projects;

public sealed class ProjectFactory : IProjectFactory
{
    public IProject Create(string title) => new Project(title);
}

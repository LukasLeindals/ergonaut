using Ergonaut.Core.Models.Project;

namespace Ergonaut.App.Features.Projects;

public interface IProjectFactory
{
    IProject Create(string title);
}

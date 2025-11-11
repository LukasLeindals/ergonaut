using Ergonaut.Core.Repositories;
using Ergonaut.Core.Models.Project;
using Ergonaut.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ergonaut.Infrastructure.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly ErgonautDbContext _db;

    public ProjectRepository(ErgonautDbContext db) => _db = db;

    public async Task<IProject?> GetAsync(Guid id, CancellationToken ct = default) =>
        await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<IProject>> ListAsync(CancellationToken ct = default) =>
        await _db.Projects.AsNoTracking()
                         .OrderBy(p => p.CreatedAt)
                         .ToListAsync(ct);

    public async Task<IProject> AddAsync(IProject project, CancellationToken ct = default)
    {
        Project? existing = await GetByNameAsync(project.Title, ct) as Project;
        if (existing is not null)
            throw new InvalidOperationException($"Project with name {project.Title} already exists.");

        _db.Projects.Add(project as Project ?? throw new NotSupportedException($"Expected {nameof(Project)} type but got {project.GetType().Name}."));
        await _db.SaveChangesAsync(ct);
        return project;
    }

    public async Task UpdateAsync(IProject project, CancellationToken ct = default)
    {
        _db.Projects.Update((Project)project);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Projects.FindAsync([id], ct);
        if (entity is null)
            return;
        _db.Projects.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IProject?> GetByNameAsync(string projectName, CancellationToken ct = default) =>
        await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Title == projectName, ct);
}

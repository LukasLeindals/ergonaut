using Project = Ergonaut.Core.Models.Project;
using Ergonaut.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ergonaut.Infrastructure.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly ErgonautDbContext _db;

    public ProjectRepository(ErgonautDbContext db) => _db = db;

    public async Task<Project?> GetAsync(Guid id, CancellationToken ct = default) =>
        await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Project>> ListAsync(CancellationToken ct = default) =>
        await _db.Projects.AsNoTracking()
                         .OrderBy(p => p.CreatedAt)
                         .ToListAsync(ct);

    public async Task<Project> AddAsync(Project project, CancellationToken ct = default)
    {
        _db.Projects.Add(project);
        await _db.SaveChangesAsync(ct);
        return project;
    }

    public async Task UpdateAsync(Project project, CancellationToken ct = default)
    {
        _db.Projects.Update(project);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Projects.FindAsync([id], ct);
        if (entity is null) return;
        _db.Projects.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }
}

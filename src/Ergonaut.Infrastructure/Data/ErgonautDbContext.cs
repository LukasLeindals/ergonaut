using Ergonaut.Core.Models.WorkItem;
using Ergonaut.Core.Models.Project;
using Microsoft.EntityFrameworkCore;

namespace Ergonaut.Infrastructure.Data;

public class ErgonautDbContext : DbContext
{
    public ErgonautDbContext(DbContextOptions<ErgonautDbContext> options)
        : base(options) { }

    public DbSet<LocalProject> Projects => Set<LocalProject>();
    public DbSet<LocalWorkItem> WorkItems => Set<LocalWorkItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LocalProject>(project =>
        {
            project.ToTable("Projects");
            project.HasKey(p => p.Id);
            project.Property(p => p.Title)
                   .IsRequired()
                   .HasMaxLength(200);
        });

        modelBuilder.Entity<LocalWorkItem>(workItem =>
        {
            workItem.ToTable("Tasks");
            workItem.HasKey(t => t.Id);
            workItem.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);
            workItem.Property(t => t.ProjectId)
                .IsRequired();
            workItem.HasOne<LocalProject>().WithMany()
                .HasForeignKey(t => t.ProjectId);
        });
    }
}

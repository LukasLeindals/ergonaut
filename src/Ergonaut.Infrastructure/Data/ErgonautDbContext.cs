using Ergonaut.Core.Models.Task;
using Ergonaut.Core.Models.Project;
using Microsoft.EntityFrameworkCore;

namespace Ergonaut.Infrastructure.Data;

public class ErgonautDbContext : DbContext
{
    public ErgonautDbContext(DbContextOptions<ErgonautDbContext> options)
        : base(options) { }

    public DbSet<LocalProject> Projects => Set<LocalProject>();
    public DbSet<LocalTask> Tasks => Set<LocalTask>();

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

        modelBuilder.Entity<LocalTask>(task =>
        {
            task.ToTable("Tasks");
            task.HasKey(t => t.Id);
            task.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);
            task.Property(t => t.ProjectId)
                .IsRequired();
            task.HasOne<LocalProject>().WithMany()
                .HasForeignKey(t => t.ProjectId);
        });
    }
}

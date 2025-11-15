using Ergonaut.Core.Models.WorkItem;
using Ergonaut.Core.Models.Project;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Ergonaut.Infrastructure.Data;

public class ErgonautDbContext : DbContext
{
    public ErgonautDbContext(DbContextOptions<ErgonautDbContext> options)
        : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(project =>
        {
            project.ToTable("Projects");
            project.HasKey(p => p.Id);
            project.Property(p => p.Title)
                   .IsRequired()
                   .HasMaxLength(200);
            project.Property(p => p.Description)
                   .HasMaxLength(1000)
                   .IsRequired(false);
            project.Property(p => p.SourceLabel)
                   .IsRequired(false);
        });



        modelBuilder.Entity<WorkItem>(workItem =>
        {
            workItem.ToTable("Tasks");
            workItem.HasKey(t => t.Id);
            workItem.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);
            workItem.Property(t => t.ProjectId)
                .IsRequired();
            workItem.HasOne<Project>().WithMany()
                .HasForeignKey(t => t.ProjectId);

            workItem.Property(t => t.SourceData)
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, _jsonOptions),
                v => string.IsNullOrWhiteSpace(v)
                    ? null
                    : JsonSerializer.Deserialize<Dictionary<string, JsonElement?>>(v, _jsonOptions))
            .HasColumnType("TEXT")
            .Metadata.SetValueComparer(
                new ValueComparer<Dictionary<string, JsonElement?>?>(
                    (c1, c2) => JsonSerializer.Serialize(c1, _jsonOptions) == JsonSerializer.Serialize(c2, _jsonOptions),
                    c => c == null ? 0 : JsonSerializer.Serialize(c, _jsonOptions).GetHashCode(),
                    c => c == null ? null : JsonSerializer.Deserialize<Dictionary<string, JsonElement?>>(JsonSerializer.Serialize(c, _jsonOptions), _jsonOptions)
                ));
        });
    }

    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
}

using FileAnalysisService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileAnalysisService.Infrastructure.Db;

public class AnalysisDbContext : DbContext
{
    public DbSet<Work> Works { get; set; } = default!;
    public DbSet<Report> Reports { get; set; } = default!;

    public AnalysisDbContext(DbContextOptions<AnalysisDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Work>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.StudentId).IsRequired();
            entity.Property(x => x.AssignmentId).IsRequired();
            entity.Property(x => x.FileId).IsRequired();
            entity.Property(x => x.SubmittedAt).IsRequired();
            entity.HasMany(x => x.Reports)
                .WithOne(r => r.Work)
                .HasForeignKey(r => r.WorkId);
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FileHash).IsRequired();
            entity.Property(x => x.CheckedAt).IsRequired();
        });
    }
}
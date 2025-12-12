using FileStorageService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileStorageService.Infrastructure.Db;

public class StorageDbContext : DbContext
{
    public DbSet<FileMetadata> Files { get; set; } = null!;

    public StorageDbContext(DbContextOptions<StorageDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FileMetadata>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.OriginalName).IsRequired().HasMaxLength(255);
            entity.Property(x => x.StoredName).IsRequired().HasMaxLength(255);
            entity.Property(x => x.ContentType).IsRequired().HasMaxLength(128);
        });
    }
}
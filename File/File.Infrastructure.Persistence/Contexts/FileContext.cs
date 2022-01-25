using File.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace File.Infrastructure.Persistence.Contexts
{
    public class FileContext : DbContext
    {
        public FileContext(DbContextOptions<FileContext> contextOptions)
            : base(contextOptions)
        {
        }

        public DbSet<ImageDetails> Images { get; set; }
        public DbSet<FileDetails> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileDetails>().ToTable("file");
            modelBuilder.Entity<FileDetails>().HasKey(fileDetails => fileDetails.FileKey);
            modelBuilder.Entity<FileDetails>().Property(fileDetails => fileDetails.FileKey).HasColumnName("file_id");
            modelBuilder.Entity<FileDetails>().Property(fileDetails => fileDetails.OwnerId).HasColumnName("owner_id");
            modelBuilder.Entity<FileDetails>().Property(fileDetails => fileDetails.FileType).HasColumnName("file_type");
            modelBuilder.Entity<FileDetails>().Property(fileDetails => fileDetails.OriginalName).HasColumnName("original_name");
            modelBuilder.Entity<FileDetails>().Property(fileDetails => fileDetails.CreatedAt).HasColumnName("created_at");
            modelBuilder.Entity<FileDetails>().Property(fileDetails => fileDetails.RemovedAt).HasColumnName("removed_at");

            modelBuilder.Entity<ImageDetails>().ToTable("image");
            modelBuilder.Entity<ImageDetails>().HasKey(imageDetails => imageDetails.ImageKey);
            modelBuilder.Entity<ImageDetails>().Property(imageDetails => imageDetails.ImageKey).HasColumnName("image_id");
            modelBuilder.Entity<ImageDetails>().Property(imageDetails => imageDetails.Size).HasColumnName("size");
            modelBuilder.Entity<ImageDetails>().HasOne(imageDetails => imageDetails.FileDetails).WithMany(fileDetails=>fileDetails.ImageDetails).HasForeignKey("file_id");
        }
    }
}

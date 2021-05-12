using FileToS3Storage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileToS3Storage.Database
{
    public class FileDbContext : DbContext
    {
        public FileDbContext(DbContextOptions<FileDbContext> options) : base(options) { }

        public DbSet<FileS3> Files { get; set; }
    }
}
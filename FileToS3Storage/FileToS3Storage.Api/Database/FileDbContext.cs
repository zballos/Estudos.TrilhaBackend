using FileToS3Storage.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FileToS3Storage.Api.Database
{
    public class FileDbContext : DbContext
    {
        public FileDbContext(DbContextOptions<FileDbContext> options) : base(options) { }

        public DbSet<FileS3> Files { get; set; }
    }
}
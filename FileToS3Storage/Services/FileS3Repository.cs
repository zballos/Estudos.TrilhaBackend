using System.Collections.Generic;
using System.Linq;
using FileToS3Storage.Database;
using FileToS3Storage.Models;
using FileToS3Storage.Services.Interfaces;

namespace FileToS3Storage.Services
{
    public class FileS3Repository : IFileS3Repository
    {
        private readonly FileDbContext _context;

        public FileS3Repository(FileDbContext context)
        {
            _context = context;
        }

        public void Add(FileS3 fileS3)
        {
            _context.Files.Add(fileS3);
            _context.SaveChanges();
        }

        public void Delete(FileS3 fileS3)
        {
            _context.Files.Remove(fileS3);
            _context.SaveChanges();
        }

        public IList<FileS3> GetAll()
        {
            return _context.Files.ToList();
        }

        public FileS3 GetById(int id)
        {
            return _context.Files.FirstOrDefault(files3 => files3.Id == id);
        }

        public void Update(FileS3 fileS3)
        {
            _context.Files.Update(fileS3);
            _context.SaveChanges();
        }
    }
}
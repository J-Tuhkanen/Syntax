using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Syntax.Data;
using Syntax.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Syntax.Services
{
    public class FileService : IFileService
    {
        private IWebHostEnvironment _environment;
        private ApplicationDbContext _appDbContext;

        public FileService(IWebHostEnvironment environment, ApplicationDbContext appDbContext)
        {
            _environment = environment;
            _appDbContext = appDbContext;
        }

        public async Task<Blob> UploadFileAsync(IFormFile Upload, string userId)
        {
            try
            {
                string file = Path.Combine(_environment.WebRootPath, "uploads", Upload.FileName);
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await Upload.CopyToAsync(fileStream);

                    var newBlob = new Blob
                    {
                        Id = Guid.NewGuid().ToString(),
                        Path = file,
                        Timestamp = DateTime.Now,
                        UserId = userId
                    };

                    await _appDbContext.Blobs.AddAsync(newBlob);
                    await _appDbContext.SaveChangesAsync();

                    return newBlob;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Syntax.Data;
using Syntax.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Syntax.Services
{
    public class FileService : IFileService
    {
        private IWebHostEnvironment _environment;
        private ApplicationDbContext _appDbContext;
        private string[] _allowedExtensions = new []{ "jpg", "jpeg", "png" };

        public FileService(IWebHostEnvironment environment, ApplicationDbContext appDbContext)
        {
            _environment = environment;
            _appDbContext = appDbContext;
        }


        /// <summary>
        /// TODO: Replace later with Azure Storage
        /// </summary>
        public async Task<Blob> UploadFileAsync(IFormFile Upload, string userId)
        {
            try
            {
                var fileExtension = Upload.FileName.Split(".").LastOrDefault().ToLower();
                
                if(_allowedExtensions.Contains(fileExtension))
                {
                    string publicPath = Path.Combine("files", userId + "." + fileExtension);
                    string fileName = Path.Combine(_environment.WebRootPath, publicPath);

                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        await Upload.CopyToAsync(fileStream);

                        var newBlob = new Blob
                        {
                            Id = Guid.NewGuid().ToString(),
                            Path = "/" + publicPath.Replace("\\", "/"),
                            Timestamp = DateTime.Now,
                            UserId = userId
                        };

                        await _appDbContext.Blobs.AddAsync(newBlob);
                        await _appDbContext.SaveChangesAsync();

                        return newBlob;
                    }
                }

                throw new Exception("Invalid file");
            }
            catch
            {
                throw;
            }
        }

    }
}

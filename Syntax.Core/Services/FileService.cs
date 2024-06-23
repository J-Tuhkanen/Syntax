using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Syntax.Core.Data;
using Syntax.Core.Helpers;
using Syntax.Core.Models;
using Syntax.Core.Services.Base;

namespace Syntax.Core.Services
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

        /// <summary>
        /// TODO: Replace later with Azure Storage
        /// </summary>
        public async Task<Blob> UploadFileAsync(IFormFile file, UserAccount user)
        {
            if(file.Length < 0)
                throw new Exception("Invalid file");

            var fileExtension = await GetFileFormat(file);

            if (fileExtension == ImageFormat.unknown)
                throw new Exception("Invalid file");

            string userFileName = Path.Combine("files", user.Id + "." + fileExtension.ToString());
            string fileName = Path.Combine(_environment.WebRootPath, userFileName);

            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);

                var newBlob = new Blob
                {
                    Path = "/" + userFileName.Replace("\\", "/"),
                    Timestamp = DateTime.Now
                };

                await _appDbContext.Blobs.AddAsync(newBlob);
                await _appDbContext.SaveChangesAsync();

                return newBlob;
            }            

            throw new Exception("Invalid file");
        }


        private async Task<ImageFormat> GetFileFormat(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                return FileHelper.GetImageFormatFromBytes(stream.ToArray());
            }
        }
    }
}

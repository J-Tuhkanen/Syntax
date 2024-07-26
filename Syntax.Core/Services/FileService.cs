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
        private ApplicationDbContext _appDbContext;

        public FileService(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        /// <summary>
        /// TODO: Replace later with Azure Storage
        /// </summary>
        public async Task<Blob> UploadFileAsync(IFormFile file, UserAccount user)
        {
            if (file.Length < 0)
                throw new Exception("Invalid file");

            var fileExtension = await GetFileFormat(file);

            if (fileExtension == ImageFormat.unknown)
                throw new Exception("Invalid file");

            string uploadsFolderName = Path.Combine("Uploads", user.UserName!.ToLower());

            var fileName = Guid.NewGuid().ToString() + "." + fileExtension;
            var filePath = Path.Combine($"{Directory.GetCurrentDirectory()}", uploadsFolderName, fileName);

            if (Directory.Exists(uploadsFolderName) == false)
            {
                Directory.CreateDirectory(uploadsFolderName);
            }

            using (var stream = new FileStream(filePath, FileMode.CreateNew))
            {
                await file.CopyToAsync(stream);

                user.ProfilePictureBlob = new Blob
                {
                    Path = fileName.Replace("\\", "/"),
                    Timestamp = DateTime.UtcNow
                };

                await _appDbContext.SaveChangesAsync();

                return user.ProfilePictureBlob;
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

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Syntax.Core.Data;
using Syntax.Core.Helpers;
using Syntax.Core.Models;
using Syntax.Core.Services.Base;

namespace Syntax.Core.Services
{
    public class WWWRootFileService : IFileService
    {
        /// <summary>
        /// TODO: Replace later with Azure Storage
        /// </summary>
        public async Task<Blob> UploadFileAsync(IFormFile file, UserAccount user)
        {            
            if (file.Length < 0)
                throw new Exception("Invalid file");

            if(file.Length > 1000000) // Image images larger than 1 MB.
                throw new Exception("File too large");

            var fileExtension = await GetFileFormat(file);

            if (fileExtension == ImageFormat.unknown)
                throw new Exception("Invalid file");

            string uploadsFolderName = Path.Combine("wwwroot", "images", user.UserName!.ToLower());

            var fileName = Guid.NewGuid().ToString() + "." + fileExtension;
            var filePath = Path.Combine($"{Directory.GetCurrentDirectory()}", uploadsFolderName, fileName);

            if (Directory.Exists(uploadsFolderName) == false)            
                Directory.CreateDirectory(uploadsFolderName);            

            using (var stream = new FileStream(filePath, FileMode.CreateNew))
            {
                await file.CopyToAsync(stream);

                return new Blob
                {
                    Path = fileName.Replace("\\", "/"),
                    Timestamp = DateTime.UtcNow
                };
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

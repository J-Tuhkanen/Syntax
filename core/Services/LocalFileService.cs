using Microsoft.AspNetCore.Http;
using Syntax.Core.Helpers;
using Syntax.Core.Models;
using Syntax.Core.Services.Base;

namespace Syntax.Core.Services
{
    /// <summary>
    /// Development fallback used when no Blob storage is configured: stores
    /// profile pictures on local disk as "{rootDirectory}/{userId}/{fileName}".
    /// Production uses <see cref="BlobFileService"/>.
    /// </summary>
    public class LocalFileService : IFileService
    {
        private readonly string _rootDirectory;

        public LocalFileService(string rootDirectory)
        {
            _rootDirectory = rootDirectory;
        }

        public async Task<Blob> UploadFileAsync(IFormFile file, UserAccount user)
        {
            var (content, format) = await ImageUploads.ReadAndValidateAsync(file);
            var fileName = ImageUploads.NewFileName(format);

            var userDirectory = Path.Combine(_rootDirectory, user.Id);
            Directory.CreateDirectory(userDirectory);

            await File.WriteAllBytesAsync(Path.Combine(userDirectory, fileName), content);

            return new Blob
            {
                Path = fileName
            };
        }

        public Task<StoredFile?> OpenReadAsync(string userId, string fileName, CancellationToken cancellationToken = default)
        {
            var path = Path.Combine(_rootDirectory, userId, fileName);

            if (!File.Exists(path))
                return Task.FromResult<StoredFile?>(null);

            Stream content = File.OpenRead(path);
            var lastModified = new DateTimeOffset(File.GetLastWriteTimeUtc(path), TimeSpan.Zero);

            return Task.FromResult<StoredFile?>(new StoredFile(content, ImageUploads.ContentType(fileName), lastModified));
        }
    }
}

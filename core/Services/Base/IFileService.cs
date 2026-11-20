using Microsoft.AspNetCore.Http;
using Syntax.Core.Models;

namespace Syntax.Core.Services.Base
{
    public interface IFileService
    {
        /// <summary>
        /// Validates and stores a profile picture for <paramref name="user"/>.
        /// The returned <see cref="Blob.Path"/> is the file name, served at /users/{userId}/{fileName}.
        /// </summary>
        Task<Blob> UploadFileAsync(IFormFile Upload, UserAccount user);

        /// <summary>
        /// Opens a stored file, or returns null when it doesn't exist.
        /// Callers must check the location with <see cref="Helpers.ImageUploads.IsValidLocation"/> first.
        /// </summary>
        Task<StoredFile?> OpenReadAsync(string userId, string fileName, CancellationToken cancellationToken = default);
    }
}

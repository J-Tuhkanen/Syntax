using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Syntax.Core.Helpers;
using Syntax.Core.Models;
using Syntax.Core.Services.Base;
using Blob = Syntax.Core.Models.Blob;

namespace Syntax.Core.Services
{
    /// <summary>
    /// Stores profile pictures in an Azure Blob container as "{userId}/{fileName}".
    /// The container client is created with a managed identity (see Startup); no
    /// account keys are involved.
    /// </summary>
    public class BlobFileService : IFileService
    {
        private readonly BlobContainerClient _container;

        public BlobFileService(BlobContainerClient container)
        {
            _container = container;
        }

        public async Task<Blob> UploadFileAsync(IFormFile file, UserAccount user)
        {
            var (content, format) = await ImageUploads.ReadAndValidateAsync(file);
            var fileName = ImageUploads.NewFileName(format);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = ImageUploads.ContentType(format),
                    CacheControl = ImageUploads.CacheControl,
                },
            };

            await _container.GetBlobClient($"{user.Id}/{fileName}").UploadAsync(new BinaryData(content), options);

            return new Blob
            {
                Path = fileName
            };
        }

        public async Task<StoredFile?> OpenReadAsync(string userId, string fileName, CancellationToken cancellationToken = default)
        {
            var blob = _container.GetBlobClient($"{userId}/{fileName}");

            try
            {
                var download = await blob.DownloadStreamingAsync(new BlobDownloadOptions(), cancellationToken);
                var details = download.Value.Details;

                return new StoredFile(
                    download.Value.Content,
                    string.IsNullOrEmpty(details.ContentType) ? ImageUploads.ContentType(fileName) : details.ContentType,
                    details.LastModified);
            }
            catch (RequestFailedException e) when (e.Status == StatusCodes.Status404NotFound)
            {
                return null;
            }
        }
    }
}

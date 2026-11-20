using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntax.Core.Helpers;
using Syntax.Core.Services.Base;

namespace Syntax.API.Controllers
{
    /// <summary>
    /// Serves uploaded profile pictures at the URL the frontend uses,
    /// /users/{userId}/{fileName}, streaming them from the configured file store
    /// (Blob storage in Azure, local disk in development).
    /// </summary>
    [ApiController]
    public class UserFilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public UserFilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [AllowAnonymous]
        [HttpGet("/users/{userId}/{fileName}")]
        public async Task<IActionResult> GetAsync(string userId, string fileName, CancellationToken cancellationToken)
        {
            if (!ImageUploads.IsValidLocation(userId, fileName))
                return NotFound();

            var file = await _fileService.OpenReadAsync(userId, fileName, cancellationToken);

            if (file == null)
                return NotFound();

            Response.Headers.CacheControl = ImageUploads.CacheControl;

            return File(file.Content, file.ContentType, file.LastModified, entityTag: null);
        }
    }
}

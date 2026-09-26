using Microsoft.AspNetCore.Http;
using Syntax.Core.Models;

namespace Syntax.Core.Services.Base
{
    public interface IFileService
    {
        Task<Blob> UploadFileAsync(IFormFile Upload, UserAccount user);
    }
}
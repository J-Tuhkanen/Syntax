using Microsoft.AspNetCore.Http;
using Syntax.Core.Models;
using System.Threading.Tasks;

namespace Syntax.Core.Services.Base
{
    public interface IFileService
    {
        Task<Blob> UploadFileAsync(IFormFile Upload, string userId);
    }
}
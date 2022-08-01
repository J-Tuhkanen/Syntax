using Microsoft.AspNetCore.Http;
using Syntax.Models;
using System.Threading.Tasks;

namespace Syntax.Services
{
    public interface IFileService
    {
        Task<Blob> UploadFileAsync(IFormFile Upload, string userId);
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Syntax.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> UploadImage([FromBody] ImageUploadRequest request)
        {
            return View();
        }
    }

    public class ImageUploadRequest
    {
        [Required]
        public IFormFile File { get; set; }
    }
}

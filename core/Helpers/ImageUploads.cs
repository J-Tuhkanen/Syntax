using Microsoft.AspNetCore.Http;

namespace Syntax.Core.Helpers
{
    /// <summary>
    /// Rules shared by every <see cref="Services.Base.IFileService"/> implementation:
    /// what an acceptable profile picture is and how stored files are named.
    /// </summary>
    public static class ImageUploads
    {
        /// <summary>Images larger than 1 MB are rejected.</summary>
        public const long MaxBytes = 1_000_000;

        /// <summary>Uploaded file names never change, so clients may cache them forever.</summary>
        public const string CacheControl = "public, max-age=31536000, immutable";

        /// <summary>
        /// Reads the upload into memory and checks its size and magic bytes.
        /// </summary>
        public static async Task<(byte[] Content, ImageFormat Format)> ReadAndValidateAsync(IFormFile file)
        {
            if (file.Length <= 0)
                throw new InvalidOperationException("Invalid file");

            if (file.Length > MaxBytes)
                throw new InvalidOperationException("File too large");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            var content = stream.ToArray();

            var format = FileHelper.GetImageFormatFromBytes(content);

            if (format == ImageFormat.unknown)
                throw new InvalidOperationException("Invalid file");

            return (content, format);
        }

        /// <summary>Stored files are named "&lt;guid&gt;.&lt;format&gt;", e.g. "3f2b...c1.png".</summary>
        public static string NewFileName(ImageFormat format) => $"{Guid.NewGuid()}.{format}";

        /// <summary>
        /// True for a user id and file name this app could have produced. Anything
        /// else (including "..", slashes and other path tricks) is rejected.
        /// </summary>
        public static bool IsValidLocation(string userId, string fileName)
        {
            if (!Guid.TryParse(userId, out _))
                return false;

            var extension = Path.GetExtension(fileName);
            var name = Path.GetFileNameWithoutExtension(fileName);
            var format = extension.TrimStart('.');

            return Guid.TryParseExact(name, "D", out _)
                && fileName == name + extension
                && format != nameof(ImageFormat.unknown)
                && Enum.GetNames<ImageFormat>().Contains(format);
        }

        public static string ContentType(ImageFormat format) => format switch
        {
            ImageFormat.bmp => "image/bmp",
            ImageFormat.jpeg => "image/jpeg",
            ImageFormat.gif => "image/gif",
            ImageFormat.tiff => "image/tiff",
            ImageFormat.png => "image/png",
            _ => "application/octet-stream",
        };

        public static string ContentType(string fileName) =>
            Enum.TryParse<ImageFormat>(Path.GetExtension(fileName).TrimStart('.'), out var format)
                ? ContentType(format)
                : "application/octet-stream";
    }
}
